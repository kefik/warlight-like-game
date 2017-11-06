namespace GameObjectsLib.GameUser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using NetworkCommObjects;
    using NetworkCommObjects.Message;
    using Players;
    using ProtoBuf;

    [Serializable]
    [ProtoContract]
    public class MyNetworkUser : NetworkUser, IDisposable
    {
        public override UserType UserType
        {
            get { return UserType.MyNetworkUser; }
        }

        private MyNetworkUser()
        {
        }

        private readonly TcpClient client;
        private readonly IPEndPoint serverEndPoint;

        public MyNetworkUser(string name, TcpClient client, IPEndPoint serverEndPoint) : base(name)
        {
            this.client = client ?? throw new ArgumentException();
            this.serverEndPoint = serverEndPoint;
        }

        [ProtoMember(1)]
        public string Email { get; set; }

        [ProtoMember(2)]
        public string Password { get; set; }

        /// <summary>
        ///     Attempts to log to the server specified in constructor.
        /// </summary>
        /// <param name="password">Password.</param>
        /// <returns>True, if this client successfully logs into the server.</returns>
        public async Task<bool> LogInAsync(string password)
        {
            if (!client.Connected)
            {
                await client.ConnectAsync(serverEndPoint.Address, serverEndPoint.Port);
            }

            NetworkStream stream = client.GetStream();

            Password = password;
            await Send(stream, new UserLogInRequestMessage
            {
                LoggingUser = this
            });
            // remove password pointer for security
            Password = null;
            GC.Collect();

            UserLogInResponseMessage responseMessage = await Receive<UserLogInResponseMessage>(stream);


            if (responseMessage == null)
            {
                return false;
            }

            if (responseMessage.SuccessfullyLoggedIn)
            {
                Email = responseMessage.Email;
            }

            return responseMessage.SuccessfullyLoggedIn;
        }

        /// <summary>
        ///     Based on client passed in constructor, finds out, whether client is still connected to the server.
        /// </summary>
        /// <returns>True, if client is connected to the server.</returns>
        public bool IsLoggedIn()
        {
            // if the tcp connection still exists, then the user is logged in, because otherwise server would close the connection
            return client.Connected;
        }

        /// <summary>
        ///     Creates a game from the seed.
        /// </summary>
        /// <returns>New games Id or null wrapped in task.</returns>
        public async Task<bool> CreateGameAsync(HumanPlayer creatingPlayer, ICollection<AiPlayer> aiPlayers,
            string mapName, int freeSlotsCount)
        {
            if (!client.Connected)
            {
                await client.ConnectAsync(serverEndPoint.Address, serverEndPoint.Port);
            }

            if (creatingPlayer.User != this)
            {
                return false;
            }

            NetworkStream stream = client.GetStream();

            await Send(stream, new CreateGameRequestMessage
            {
                AiPlayers = aiPlayers,
                CreatingPlayer = creatingPlayer,
                FreeSlotsCount = freeSlotsCount,
                MapName = mapName
            });

            CreateGameResponseMessage answer = await Receive<CreateGameResponseMessage>(stream);

            return answer != null && answer.Successful;
        }

        /// <summary>
        ///     Finds out list of games this user is signed to play.
        /// </summary>
        /// <returns>List of games this user plays.</returns>
        public async Task<IEnumerable<GameHeaderMessageObject>> GetListOfMyGamesAsync()
        {
            if (!client.Connected)
            {
                await client.ConnectAsync(serverEndPoint.Address, serverEndPoint.Port);
            }

            NetworkStream stream = client.GetStream();
            {
                await Send(stream, new LoadMyGamesListRequestMessage
                {
                    RequestingUser = this
                });
            }
            {
                LoadMyGamesListResponseMessage answer = await Receive<LoadMyGamesListResponseMessage>(stream);

                return answer?.GameHeaderMessageObjects;
            }
        }

        /// <summary>
        ///     Gets list of opened games on the server this user is connected to.
        /// </summary>
        /// <returns>List of opened games on the server.</returns>
        public async Task<IEnumerable<OpenedGameHeaderMessageObject>> GetListOfOpenedGamesAsync()
        {
            if (!client.Connected)
            {
                await client.ConnectAsync(serverEndPoint.Address, serverEndPoint.Port);
            }

            NetworkStream stream = client.GetStream();

            await Send(stream, new LoadOpenedGamesListRequestMessage
            {
                RequestingUser = this
            });

            LoadOpenedGamesListResponseMessage answer = await Receive<LoadOpenedGamesListResponseMessage>(stream);
            return answer?.GameHeaderMessageObjects;
        }

        /// <summary>
        ///     Asynchronously loads opened game request.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="gameId">Id of the game.</param>
        /// <returns></returns>
        public async Task<bool> JoinOpenedGameAsync(HumanPlayer player, int gameId)
        {
            if (!client.Connected)
            {
                await client.ConnectAsync(serverEndPoint.Address, serverEndPoint.Port);
            }

            NetworkStream stream = client.GetStream();

            await Send(stream, new JoinGameRequestMessage
            {
                OpenedGameId = gameId,
                RequestingPlayer = player
            });

            JoinGameResponseMessage answer = await Receive<JoinGameResponseMessage>(stream);
            return answer == null ? true : false;
        }

        /// <summary>
        ///     Asynchronously logs user out.
        /// </summary>
        /// <returns>True, if player was successfully logged off.</returns>
        public async Task<bool> LogOut()
        {
            if (client.Connected)
            {
                return await Task.Factory.StartNew(() => client.Client.DisconnectAsync(new SocketAsyncEventArgs
                {
                    DisconnectReuseSocket = true
                }), TaskCreationOptions.DenyChildAttach);
            }
            return false;
        }

        public async Task LoadOpenedGame(int id)
        {
            if (!client.Connected)
            {
                await client.ConnectAsync(serverEndPoint.Address, serverEndPoint.Port);
            }

            NetworkStream stream = client.GetStream();
            {
                SerializationObjectWrapper wrapper = new SerializationObjectWrapper<LoadOpenedGamesListRequestMessage>
                {
                    TypedValue = new LoadOpenedGamesListRequestMessage
                    {
                        RequestingUser = this
                    }
                };
                await wrapper.SerializeAsync(stream);
            }
            throw new NotImplementedException();
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

        public async Task LoadStartedGame(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(false);
        }

        private bool disposed;

        private void Dispose(bool calledFromFinalizer)
        {
            if (disposed == false)
            {
                disposed = true;
                client?.Close();
                if (calledFromFinalizer == false)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }

        ~MyNetworkUser()
        {
            Dispose(true);
        }
    }
}
