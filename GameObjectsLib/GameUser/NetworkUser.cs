namespace GameObjectsLib.GameUser
{
    using System;
    using ProtoBuf;

    [Serializable]
    [ProtoInclude(200, typeof(MyNetworkUser))]
    [ProtoContract]
    public class NetworkUser : User
    {
        protected NetworkUser()
        {
        }

        public override UserType UserType
        {
            get { return UserType.NetworkUser; }
        }

        public NetworkUser(string name) : base(name)
        {
        }
    }
}