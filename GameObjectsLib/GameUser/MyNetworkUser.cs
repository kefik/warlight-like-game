using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using GameObjectsLib.Game;
using GameObjectsLib.NetworkCommObjects;
using ProtoBuf;

namespace GameObjectsLib.GameUser
{
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
        public bool LogIn(string password)
        {
            if (!client.Connected) client.Connect(serverEndPoint);

            var stream = client.GetStream();

            Password = password;
            NetworkObjectWrapper wrapper = new NetworkObjectWrapper<MyNetworkUser>() { TypedValue = this };
            wrapper.Serialize(stream);
            // remove password pointer for security
            Password = null;
            GC.Collect();

            bool successful = (bool)NetworkObjectWrapper.Deserialize(stream).Value;
            if (successful)
            {
                var user = (MyNetworkUser)NetworkObjectWrapper.Deserialize(stream).Value;
                Email = user.Email;
            }

            return successful;
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
        /// <param name="seed">Seed of the game.</param>
        /// <returns>True, if everything ran correctly and the game was created.</returns>
        public bool CreateGame(GameSeed seed)
        {
            if (!client.Connected) client.Connect(serverEndPoint);

            var stream = client.GetStream();
            {
                NetworkObjectWrapper wrapper = new NetworkObjectWrapper<GameSeed>() {TypedValue = seed};
                wrapper.Serialize(stream);
            }
            {
                var answer = NetworkObjectWrapper.Deserialize(stream).Value;
                if (answer is bool)
                {
                    bool wasCreatedCorrectly = (bool) answer;
                    return wasCreatedCorrectly;
                }
                return false;
            }
        }

        public bool LogOut()
        {
            if (client.Connected)
            {
                client.Client.Disconnect(true);
                return true;
            }
            return false;
        }

        public bool ChangePassword(string oldPassword, string newPassword)
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
