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
    [ProtoInclude(200, typeof(MyNetworkUser))]
    [ProtoContract]
    public class NetworkUser : User
    {
        protected NetworkUser() : base() { }

        public override UserType UserType
        {
            get { return UserType.NetworkUser; }
        }

        public NetworkUser(string name) : base(name)
        {
        }
        
    }
}