using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace GameObjectsLib.NetworkCommObjects.Message
{
    using GameUser;

    /// <summary>
    /// Represents response from the server on create game attempt.
    /// </summary>
    [ProtoContract]
    public class CreateGameResponseMessage
    {
        /// <summary>
        /// Id of the game that was successfully created or null.
        /// </summary>
        [ProtoMember(1)]
        public int? Id { get; set; }
    }
}
