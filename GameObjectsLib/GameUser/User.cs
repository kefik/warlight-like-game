using System;
using System.Dynamic;
using System.Runtime.Serialization;
using ProtoBuf;

namespace GameObjectsLib.GameUser
{
    public enum UserType
    {
        None, LocalUser, NetworkUser, MyNetworkUser
    }
    /// <summary>
    /// Represents user that controls player in the game.
    /// </summary>
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(50, typeof(NetworkUser))]
    [ProtoInclude(51, typeof(LocalUser))]
    [ProtoInclude(52, typeof(MyNetworkUser))]
    public abstract class User
    {
        public string Name { get; set; }

        public abstract UserType UserType { get; }

        protected User(string name)
        {
            Name = name;
        }
        protected User() { }
    }
    
}
