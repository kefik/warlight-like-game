﻿using System;
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
    [ProtoInclude(100, typeof(NetworkUser))]
    [ProtoInclude(101, typeof(LocalUser))]
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
