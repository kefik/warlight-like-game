using System;
using ProtoBuf;

namespace GameObjectsLib.GameUser
{
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class MyNetworkUser : NetworkUser
    {
        public override UserType UserType
        {
            get { return UserType.MyNetworkUser; }
        }

        public MyNetworkUser(int id, string name) : base(id, name) { }

        public bool LogIn(string password)
        {
            throw new NotImplementedException();
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
