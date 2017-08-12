using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace GameObjectsLib.NetworkCommObjects.Message
{
    [ProtoContract]
    public class CreateGameResponseMessage
    {
        [ProtoMember(1)]
        public Game.Game Game { get; set; }
    }
}
