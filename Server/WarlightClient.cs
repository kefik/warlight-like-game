namespace Server.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Entities;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameUser;
    using GameObjectsLib.NetworkCommObjects;
    using GameObjectsLib.NetworkCommObjects.Message;
    using GameObjectsLib.Player;
    using User = Entities.User;

    internal class WarlightClient : IDisposable
    {
        private readonly TcpClient client;
        private CancellationToken token;

        public WarlightClient(TcpClient client, CancellationToken token)
        {
            this.client = client ?? throw new ArgumentNullException();
            this.token = token;
        }

        public async Task RunAsync()
        {
            try
            {
                bool isLoggedIn = false;

                while (!token.IsCancellationRequested)
                {
                    NetworkStream stream = client.GetStream();

                    Task delayTask = Task.Delay(TimeSpan.FromSeconds(50000));
                    Task<SerializationObjectWrapper> deserializationTask =
                        SerializationObjectWrapper.DeserializeAsync(stream);
                    Task completedTask = await Task.WhenAny(delayTask, deserializationTask);

                    // client didnt send anything
                    if (completedTask == delayTask)
                    {
                        Dispose();
                        throw new ClientNotRespondingException();
                    }
                    // its non blocking now
                    object deserializedObject = deserializationTask.Result.Value;

                    await TypeSwitch.DoAsync(
                        deserializedObject,
                        TypeSwitch.Case<UserLogInRequestMessage>(async message =>
                        {
                            MyNetworkUser user = message.LoggingUser;

                            string passwordHash;
                            {
                                // calculate hash
                                byte[] data = Encoding.ASCII.GetBytes(user.Password);
                                data = new SHA256Managed().ComputeHash(data);
                                passwordHash = Encoding.ASCII.GetString(data);
                            }
                            using (WarlightDbContext db = new WarlightDbContext())
                            {
                                var matchedUser = (from dbUser in db.Users
                                                    where (dbUser.Name == user.Name) &&
                                                          (dbUser.PasswordHash == passwordHash)
                                                    select dbUser).AsEnumerable().FirstOrDefault();
                                bool existsMatchingUser = matchedUser != null;
                                SerializationObjectWrapper userWrapper =
                                    new SerializationObjectWrapper<UserLogInResponseMessage>
                                    {
                                        TypedValue = new UserLogInResponseMessage
                                        {
                                            SuccessfullyLoggedIn = existsMatchingUser,
                                            Email = matchedUser?.Email
                                        }
                                    };
                                await userWrapper.SerializeAsync(stream);

                                isLoggedIn = existsMatchingUser;
                            }
                        }),
                        TypeSwitch.Case<CreateGameRequestMessage>(async message =>
                        {
                            async Task RequestUnsuccessfulResponse(Stream responseStream)
                            {
                                SerializationObjectWrapper response
                                    = new SerializationObjectWrapper<CreateGameResponseMessage>
                                    {
                                        TypedValue = new CreateGameResponseMessage
                                        {
                                            Successful = false
                                        }
                                    };
                                await response.SerializeAsync(responseStream);
                            }

                            async Task RequestSuccessfulResponse(Stream responseStream)
                            {
                                SerializationObjectWrapper response
                                    = new SerializationObjectWrapper<CreateGameResponseMessage>
                                    {
                                        TypedValue = new CreateGameResponseMessage
                                        {
                                            Successful = true
                                        }
                                    };
                                await response.SerializeAsync(responseStream);
                            }

                            if ((isLoggedIn == false) || (message.FreeSlotsCount == 0))
                            {
                                await RequestUnsuccessfulResponse(stream);
                                return;
                            }

                            using (WarlightDbContext db = new WarlightDbContext())
                            {
                                MapInfo mapInfo = db.GetMatchingMap(message.MapName);
                                if (mapInfo == null)
                                {
                                    await RequestUnsuccessfulResponse(stream);
                                    return;
                                }

                                Map map = new Map(mapInfo.Id,
                                    mapInfo.Name,
                                    mapInfo.PlayersLimit, mapInfo.TemplatePath);

                                HumanPlayer creatingPlayer = message.CreatingPlayer;
                                GameObjectsLib.GameUser.User creatingUser = creatingPlayer.User;

                                User userInfo = db.GetMatchingUser(creatingUser.Name);
                                if (userInfo == null)
                                {
                                    await RequestUnsuccessfulResponse(stream);
                                    return;
                                }

                                int newGameId = db.GetMaxOpenedGameId() + 1;

                                var players = new List<Player>
                                {
                                    creatingPlayer
                                };

                                ICollection<AiPlayer> aiPlayers = message.AiPlayers ?? new List<AiPlayer>();
                                players.AddRange(aiPlayers);

                                GameFactory factory = new GameFactory();
                                Game game =factory.CreateGame(newGameId, GameType.MultiplayerNetwork, map, players, fogOfWar: true);

                                OpenedGame openedGame = new OpenedGame
                                {
                                    Id = newGameId,
                                    MapName = map.Name,
                                    AiPlayersCount = aiPlayers.Count,
                                    HumanPlayersCount = 1,
                                    OpenedSlotsNumber = message.FreeSlotsCount,
                                    SignedUsers = new HashSet<Entities.User>
                                    {
                                        userInfo
                                    },
                                    GameCreatedDateTime = DateTime.Now.ToString()
                                };

                                openedGame.SetGame(game);
                                userInfo.OpenedGames.Add(openedGame);
                                db.OpenedGames.Add(openedGame);

                                await db.SaveChangesAsync();

                                await RequestSuccessfulResponse(stream);
                            }
                        }),
                        TypeSwitch.Case<LoadMyGamesListRequestMessage>(async message =>
                        {
                            MyNetworkUser user = message.RequestingUser;

                            using (WarlightDbContext db = new WarlightDbContext())
                            {
                                var matchingUser = db.GetMatchingUser(user.Name);

                                IEnumerable<OpenedGame> openedGames = from openedGame in db.OpenedGames.AsEnumerable()
                                                                      where openedGame.SignedUsers.Contains(
                                                                          matchingUser)
                                                                      select openedGame;
                                IEnumerable<StartedGame> startedGames =
                                    from startedGame in db.StartedGames.AsEnumerable()
                                    where startedGame.PlayingUsers.Contains(matchingUser)
                                    select startedGame;

                                var result = new List<GameHeaderMessageObject>();
                                foreach (OpenedGame openedGame in openedGames)
                                {
                                    result.Add(new OpenedGameHeaderMessageObject
                                    {
                                        GameId = openedGame.Id,
                                        AiPlayersCount = openedGame.AiPlayersCount,
                                        HumanPlayersCount = openedGame.HumanPlayersCount,
                                        MapName = openedGame.MapName,
                                        GameCreated = DateTime.Parse(openedGame.GameCreatedDateTime)
                                    });
                                }
                                foreach (StartedGame startedGame in startedGames)
                                {
                                    result.Add(new StartedGameHeaderMessageObject
                                    {
                                        GameId = startedGame.Id,
                                        AiPlayersCount = startedGame.AiPlayersCount,
                                        HumanPlayersCount = startedGame.HumanPlayersCount,
                                        MapName = startedGame.MapName,
                                        GameStarted = DateTime.Parse(startedGame.GameStartedDateTime),
                                        RoundStarted = DateTime.Parse(startedGame.LastRound.RoundStartedDateTime)
                                    });
                                }

                                {
                                    SerializationObjectWrapper wrapper =
                                        new SerializationObjectWrapper<LoadMyGamesListResponseMessage>
                                        {
                                            TypedValue = new LoadMyGamesListResponseMessage
                                            {
                                                GameHeaderMessageObjects = result
                                            }
                                        };
                                    await wrapper.SerializeAsync(stream);
                                }
                            }
                        }),
                        TypeSwitch.Case<LoadOpenedGamesListRequestMessage>(async message =>
                        {
                            MyNetworkUser user = message.RequestingUser;

                            using (WarlightDbContext db = new WarlightDbContext())
                            {
                                if (db.GetMatchingUser(user.Name) == null)
                                {
                                    return;
                                }

                                IQueryable<OpenedGame> openedGames = from openedGame in db.OpenedGames
                                                                     from dbUser in openedGame.SignedUsers
                                                                     where dbUser.Name != user.Name
                                                                     select openedGame;

                                {
                                    SerializationObjectWrapper wrapper =
                                        new SerializationObjectWrapper<LoadOpenedGamesListResponseMessage>
                                        {
                                            TypedValue = new LoadOpenedGamesListResponseMessage
                                            {
                                                GameHeaderMessageObjects = openedGames.AsEnumerable().Select(
                                                    x => new OpenedGameHeaderMessageObject
                                                    {
                                                        AiPlayersCount = x.AiPlayersCount,
                                                        GameCreated = DateTime.Parse(x.GameCreatedDateTime),
                                                        GameId = x.Id,
                                                        HumanPlayersCount = x.HumanPlayersCount,
                                                        MapName = x.MapName
                                                    })
                                            }
                                        };
                                    await wrapper.SerializeAsync(stream);
                                }
                            }
                        }),
                        TypeSwitch.Case<JoinGameRequestMessage>(async message =>
                        {
                            HumanPlayer player = message.RequestingPlayer;

                            using (WarlightDbContext db = new WarlightDbContext())
                            {
                                var matchingUser = db.GetMatchingUser(player.User.Name);

                                if (matchingUser == null)
                                {
                                    await Send(stream, new JoinGameResponseMessage
                                    {
                                        SuccessfullyJoined = false
                                    });
                                    return;
                                }

                                OpenedGame matchingOpenedGame = db.GetMatchingOpenedGame(message.OpenedGameId);

                                if (matchingOpenedGame == null)
                                {
                                    await Send(stream, new JoinGameResponseMessage
                                    {
                                        SuccessfullyJoined = false
                                    });
                                    return;
                                }

                                Game openedGame = await matchingOpenedGame.GetGameAsync();

                                matchingOpenedGame.SignedUsers.Add(matchingUser);
                                openedGame.Players.Add(player);

                                matchingOpenedGame.SetGame(openedGame);
                                matchingOpenedGame.OpenedSlotsNumber--;
                                matchingOpenedGame.HumanPlayersCount++;

                                await db.SaveChangesAsync();

                                await Send(stream, new JoinGameResponseMessage
                                {
                                    SuccessfullyJoined = true
                                });

                                if (matchingOpenedGame.OpenedSlotsNumber == 0)
                                {
                                    // TODO: start the game
                                    db.OpenedGames.Remove(matchingOpenedGame);

                                    StartedGame startedGame = new StartedGame
                                    {
                                        AiPlayersCount = matchingOpenedGame.AiPlayersCount,
                                        GameStartedDateTime = DateTime.Now.ToString(),
                                        HumanPlayersCount = matchingOpenedGame.HumanPlayersCount,
                                        MapName = matchingOpenedGame.MapName,
                                        PlayingUsers = matchingOpenedGame.SignedUsers
                                    };
                                }
                            }
                        })
                    );

                    // find out the type
                }
            }
            finally
            {
                Dispose();
            }
        }


        private async Task Send<T>(Stream stream, T value)
        {
            SerializationObjectWrapper wrapper = new SerializationObjectWrapper<T>
            {
                TypedValue = value
            };
            await wrapper.SerializeAsync(stream);
        }

        private async Task<T> Receive<T>(Stream stream) where T : class
        {
            object result = (await SerializationObjectWrapper.DeserializeAsync(stream)).Value;
            if (result.GetType() == typeof(T))
            {
                return (T) result;
            }
            return default(T);
        }

        private bool isDisposed;

        public void Dispose()
        {
            Dispose(false);
        }

        private void Dispose(bool calledFromFinalizer)
        {
            if (calledFromFinalizer == false)
            {
                GC.SuppressFinalize(this);
            }
            if (isDisposed == false)
            {
                client.Close();
                isDisposed = true;
            }
        }

        ~WarlightClient()
        {
            Dispose(true);
        }
    }
}

