using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace GameObjectsLib.NetworkCommObjects.Message
{
    /// <summary>
    /// Gives response whether the request was valid.
    /// </summary>
    [ProtoContract]
    class LoadMyGamesResponseMessage
    {
        [ProtoMember(1)]
        public bool ValidRequest { get; set; }
    }
}
