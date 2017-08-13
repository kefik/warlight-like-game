using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using GameObjectsLib.Game;
using GameObjectsLib.NetworkCommObjects;
using ProtoBuf;

namespace GameObjectsLib.GameUser
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NetworkCommObjects.Message;

    [Serializable]
    [ProtoContract]
    public class MyNetworkUser : NetworkUser, IDisposable
    {
        public override UserType UserType
        {
            get { return UserType.MyNetworkUser; }
        }
        MyNetworkUser() : base() { }

        readonly TcpClient client;
        readonly IPEndPoint serverEndPoint;

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
        /// Attempts to log to the server specified in constructor.
        /// </summary>
        /// <param name="password">Password.</param>
        /// <returns>True, if this client successfully logs into the server.</returns>
        public async Task<bool> LogInAsync(string password)
        {
            if (!client.Connected) await client.ConnectAsync(serverEndPoint.Address, serverEndPoint.Port);

            var stream = client.GetStream();

            Password = password;
            SerializationObjectWrapper wrapper =
                new SerializationObjectWrapper<UserLogInRequestMessage>()
                {
                    TypedValue = new UserLogInRequestMessage()
                    {
                        LoggingUser = this
                    }
                };
            await wrapper.SerializeAsync(stream);
            // remove password pointer for security
            Password = null;
            GC.Collect();

            var responseMessage = (await SerializationObjectWrapper.DeserializeAsync(stream)).Value as UserLogInResponseMessage;

            if (responseMessage == null) return false;

            if (responseMessage.SuccessfullyLoggedIn)
            {
                Email = responseMessage.Email;
            }

            return responseMessage.SuccessfullyLoggedIn;
        }

        /// <summary>
        /// Based on client passed in constructor, finds out, whether client is still connected to the server.
        /// </summary>
        /// <returns>True, if client is connected to the server.</returns>
        public bool IsLoggedIn()
        {
            // if the tcp connection still exists, then the user is logged in, because otherwise server would close the connection
            return client.Connected;
        }

        /// <summary>
        /// Creates a game from the seed.
        /// </summary>
        /// <returns>True, if everything ran correctly and the game was created.</returns>
        public async Task<bool> CreateGameAsync(HumanPlayer creatingPlayer, ICollection<AIPlayer> aiPlayers, string mapName, int freeSlotsCount)
        {
            if (!client.Connected) await client.ConnectAsync(serverEndPoint.Address, serverEndPoint.Port);

            if (creatingPlayer.User != this) return false;

            var stream = client.GetStream();
            {
                SerializationObjectWrapper wrapper = new SerializationObjectWrapper<CreateGameRequestMessage>()
                {
                    TypedValue = new CreateGameRequestMessage()
                    {
                        AIPlayers = aiPlayers,
                        CreatingPlayer = creatingPlayer,
                        FreeSlotsCount = freeSlotsCount,
                        MapName = mapName
                    }
                };
                await wrapper.SerializeAsync(stream);
            }
            {
                var answer = (await SerializationObjectWrapper.DeserializeAsync(stream)).Value;
                if (answer is bool)
                {
                    bool wasCreatedCorrectly = (bool) answer;
                    return wasCreatedCorrectly;
                }
                return false;
            }
        }

        public async Task<bool> LogOut()
        {
            if (client.Connected)
            {
                return await Task.Factory.StartNew(() => client.Client.DisconnectAsync(new SocketAsyncEventArgs()
                {
                    DisconnectReuseSocket = true
                }), TaskCreationOptions.DenyChildAttach);
            }
            return false;
        }

        public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(false);
        }

        bool disposed;
        void Dispose(bool calledFromFinalizer)
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
