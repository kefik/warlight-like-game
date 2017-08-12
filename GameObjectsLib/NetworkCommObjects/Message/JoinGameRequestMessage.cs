using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectsLib.GameUser;
using ProtoBuf;

namespace GameObjectsLib.NetworkCommObjects.Message
{
    [ProtoContract]
    public class JoinGameRequestMessage
    {
        [ProtoMember(1)]
        public MyNetworkUser RequestingUser { get; set; }
        [ProtoMember(2)]
        public int GameId { get; set; }
    }
}
