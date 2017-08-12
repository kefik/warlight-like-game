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
    public class CreateGameRequestMessage
    {
        [ProtoMember(1)]
        public MyNetworkUser CreatingUser { get; set; }
        [ProtoMember(2)]
        public ICollection<AIPlayer> AIPlayers { get; set; }
        [ProtoMember(3)]
        public string MapName { get; set; }
        [ProtoMember(3)]
        public int FreeSlots { get; set; }
    }
}
