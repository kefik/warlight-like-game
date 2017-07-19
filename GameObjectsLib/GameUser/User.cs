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
    [ProtoInclude(500, typeof(NetworkUser))]
    [ProtoInclude(501, typeof(LocalUser))]
    [ProtoInclude(502, typeof(MyNetworkUser))]
    public abstract class User// : ISerializable
    {
        public string Name { get; set; }

        public abstract UserType UserType { get; }

        protected User(string name)
        {
            Name = name;
        }
        protected User() { }
        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue(nameof(Name), Name, typeof(string));
        //}

        /// <summary>
        /// Takes care of deserializing.
        /// </summary>
        //protected User(SerializationInfo info, StreamingContext context)
        //{
        //    Name = (string)info.GetValue(nameof(Name), typeof(string));
        //}
    }
    
}
