using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
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

        public MyNetworkUser(int id, string name, TcpClient connectedClient, IPEndPoint serverEndPoint) : base(id, name)
        {
            if (client == null) throw new ArgumentException();

            this.client = connectedClient;
            this.serverEndPoint = serverEndPoint;
        }
        [ProtoMember(1)]
        public string Email { get; private set; }
        [ProtoMember(2)]
        public string PasswordHash { get; private set; }
        public bool LogIn(string password)
        {
            if (!client.Connected) client.Connect(serverEndPoint);

            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
                data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                PasswordHash = System.Text.Encoding.ASCII.GetString(data);
            }
            var stream = client.GetStream();
            NetworkObjectWrapper wrapper = new NetworkObjectWrapper<MyNetworkUser>() {TypedValue = this};
            wrapper.Serialize(stream);

            bool successful = (bool)NetworkObjectWrapper.Deserialize(stream).Value;
            if (successful)
            {
                var user = (MyNetworkUser) NetworkObjectWrapper.Deserialize(stream).Value;
                Email = user.Email;
            }

            return successful;
        }

        public bool LogOut()
        {
            throw new NotImplementedException();
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
            if (disposed == false && client.Connected)
            {
                disposed = true;
                client.Close();
                if (calledFromFinalizer == false)
                {
                    GC.SuppressFinalize(this);
                    GC.SuppressFinalize(client);
                }
            }
        }

        ~MyNetworkUser()
        {
            Dispose(true);
        }
    }
}
