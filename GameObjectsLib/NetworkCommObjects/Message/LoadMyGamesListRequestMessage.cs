using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectsLib.GameUser;
using ProtoBuf;

namespace GameObjectsLib.NetworkCommObjects.Message
{
    /// <summary>
    /// Loads list of games started by the specified <see cref="RequestingUser"/>
    /// </summary>
    [ProtoContract]
    public class LoadMyGamesListRequestMessage
    {
        [ProtoMember(1)]
        public MyNetworkUser RequestingUser { get; set; }
    }
}
