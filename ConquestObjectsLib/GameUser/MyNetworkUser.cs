using System;

namespace ConquestObjectsLib.GameUser
{
    public class MyNetworkUser : NetworkUser
    {
        public override UserType UserType
        {
            get { return UserType.Network; }
        }

        public MyNetworkUser(string name) : base(name) { }

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
