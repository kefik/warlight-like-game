using System;
using System.IO;
using ProtoBuf;

namespace GameObjectsLib.GameUser
{
    [Serializable]
    [ProtoContract]
    public class MyNetworkUser : NetworkUser
    {
        public override UserType UserType
        {
            get { return UserType.MyNetworkUser; }
        }
        MyNetworkUser() : base() { }

        public MyNetworkUser(int id, string name) : base(id, name) { }
        [ProtoMember(1)]
        public string Email { get; private set; }
        [ProtoMember(2)]
        public string PasswordHash { get; private set; }
        public bool LogIn(string password, Stream stream)
        {
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
                data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                PasswordHash = System.Text.Encoding.ASCII.GetString(data);
            }

            NetworkObjectWrapper wrapper = new NetworkObjectWrapper<MyNetworkUser>() {TypedValue = this};
            wrapper.Serialize(stream);

            return (bool)NetworkObjectWrapper.Deserialize(stream).Value;
        }

        public bool LogOut()
        {
            throw new NotImplementedException();
        }

        public bool ChangePassword(string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }
    }
}
