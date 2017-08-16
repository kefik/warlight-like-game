namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameUser;
    using GameObjectsLib.NetworkCommObjects;
    using GameObjectsLib.NetworkCommObjects.Message;
    using WarlightLikeDatabase;
    using Game = GameObjectsLib.Game.Game;
    using User = WarlightLikeDatabase.User;


    class WarlightClient : IDisposable
    {
        readonly TcpClient client;
        CancellationToken token;

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
                    var deserializationTask = SerializationObjectWrapper.DeserializeAsync(stream);
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
                                var data = Encoding.ASCII.GetBytes(user.Password);
                                data = new SHA256Managed().ComputeHash(data);
                                passwordHash = Encoding.ASCII.GetString(data);
                            }
                            using (WarlightDbContext db = new WarlightDbContext())
                            {
                                User matchedUser = (from dbUser in db.Users
                                                    where dbUser.Login == user.Name &&
                                                          dbUser.PasswordHash == passwordHash
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

                                if (isLoggedIn == false || message.FreeSlotsCount == 0)
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

                                    var map = GameObjectsLib.GameMap.Map.Create(mapInfo.MapInfoId,
                                        mapInfo.Name,
                                        mapInfo.PlayersLimit, mapInfo.TemplatePath);

                                    var creatingPlayer = message.CreatingPlayer;
                                    var creatingUser = creatingPlayer.User;

                                    var userInfo = db.GetMatchingUser(creatingUser.Name);
                                    if (userInfo == null)
                                    {
                                        await RequestUnsuccessfulResponse(stream);
                                        return;
                                    };

                                    int newGameId = db.GetMaxOpenedGameId() + 1;

                                    var players = new List<Player>()
                                    {
                                        creatingPlayer
                                    };

                                    var aiPlayers = message.AiPlayers ?? new List<AiPlayer>();
                                    players.AddRange(aiPlayers);

                                    var game = Game.Create(newGameId, GameType.MultiplayerNetwork, map, players);

                                    OpenedGame openedGame = new OpenedGame
                                    {
                                        OpenedGameId = newGameId,
                                        MapName = map.Name,
                                        AiPlayersCount = aiPlayers.Count,
                                        HumanPlayersCount = 1,
                                        OpenedSlotsNumber = message.FreeSlotsCount,
                                        SignedUsers = new HashSet<User>()
                                            {
                                                userInfo
                                            },
                                        GameCreatedDateTime = DateTime.Now.ToString()
                                    };

                                    using (var ms = await game.GetStreamForSerializedGameAsync())
                                    {
                                        await db.SaveGameAsync(openedGame, ms);
                                    }

                                    await RequestSuccessfulResponse(stream);
                                }


                            }),
                        TypeSwitch.Case<LoadMyGamesListRequestMessage>(async message =>
                        {
                            var user = message.RequestingUser;

                            using (var db = new WarlightDbContext())
                            {
                                var matchingUser = db.GetMatchingUser(user.Name);

                                var openedGames = from openedGame in db.OpenedGames.AsEnumerable()
                                                  where openedGame.SignedUsers.Contains(matchingUser)
                                                  select openedGame;
                                var startedGames = from startedGame in db.StartedGames.AsEnumerable()
                                                   where startedGame.PlayingUsers.Contains(matchingUser)
                                                   select startedGame;

                                var result = new List<GameHeaderMessageObject>();
                                foreach (var openedGame in openedGames)
                                {
                                    result.Add(new OpenedGameHeaderMessageObject()
                                    {
                                        GameId = openedGame.OpenedGameId,
                                        AiPlayersCount = openedGame.AiPlayersCount,
                                        HumanPlayersCount = openedGame.HumanPlayersCount,
                                        MapName = openedGame.MapName,
                                        GameCreated = DateTime.Parse(openedGame.GameCreatedDateTime)
                                    });
                                }
                                foreach (var startedGame in startedGames)
                                {
                                    result.Add(new StartedGameHeaderMessageObject()
                                    {
                                        GameId = startedGame.StartedGameId,
                                        AiPlayersCount = startedGame.AiPlayersCount,
                                        HumanPlayersCount = startedGame.HumanPlayersCount,
                                        MapName = startedGame.MapName,
                                        GameStarted = DateTime.Parse(startedGame.GameStartedDateTime),
                                        RoundStarted = DateTime.Parse(startedGame.LastRound.RoundStartedDateTime)
                                    });
                                }
                                
                                {
                                    SerializationObjectWrapper wrapper = new SerializationObjectWrapper<LoadMyGamesListResponseMessage>()
                                    {
                                        TypedValue = new LoadMyGamesListResponseMessage()
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
                            var user = message.RequestingUser;
                            
                            using (var db = new WarlightDbContext())
                            {
                                if (db.GetMatchingUser(user.Name) == null) return;

                                var openedGames = db.OpenedGames;
                                
                                {
                                    SerializationObjectWrapper wrapper = new SerializationObjectWrapper<LoadOpenedGamesListResponseMessage>()
                                    {
                                        TypedValue = new LoadOpenedGamesListResponseMessage()
                                        {
                                            GameHeaderMessageObjects = openedGames.AsEnumerable().Select(x => new OpenedGameHeaderMessageObject()
                                            {
                                                AiPlayersCount = x.AiPlayersCount,
                                                GameCreated = DateTime.Parse(x.GameCreatedDateTime),
                                                GameId = x.OpenedGameId,
                                                HumanPlayersCount = x.HumanPlayersCount,
                                                MapName = x.MapName
                                            })
                                        }
                                    };
                                    await wrapper.SerializeAsync(stream);
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

        bool isDisposed;

        public void Dispose()
        {
            Dispose(false);
        }

        void Dispose(bool calledFromFinalizer)
        {
            if (calledFromFinalizer == false) GC.SuppressFinalize(this);
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

