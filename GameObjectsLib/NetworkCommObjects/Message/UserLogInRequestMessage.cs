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
    /// Attempts to log in to the server.
    /// </summary>
    [ProtoContract]
    public class UserLogInRequestMessage
    {
        [ProtoMember(1)]
        public MyNetworkUser LoggingUser { get; set; }
    }
}
