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
    /// Attempt to join the game specified by GameId with RequestingUser.
    /// </summary>
    [ProtoContract]
    public class JoinGameRequestMessage
    {
        /// <summary>
        /// Represents player attempting to join the game.
        /// </summary>
        [ProtoMember(1)]
        public HumanPlayer RequestingPlayer { get; set; }
        [ProtoMember(2)]
        public int OpenedGameId { get; set; }
    }
}
