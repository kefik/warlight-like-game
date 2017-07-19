using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace GameObjectsLib.GameUser
{
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class NetworkUser : User
    {
        public int Id { get; }

        public override UserType UserType
        {
            get { return UserType.NetworkUser; }
        }

        public NetworkUser(int id, string name) : base(name)
        {
            Id = id;
        }
    }
}