namespace GameObjectsLib.NetworkCommObjects.Message
{
    using System.Collections.Generic;
    using ProtoBuf;

    /// <summary>
    ///     Represents request from client to server attempting to create the game.
    /// </summary>
    [ProtoContract]
    public class CreateGameRequestMessage
    {
        [ProtoMember(1)]
        public HumanPlayer CreatingPlayer { get; set; }

        [ProtoMember(2)]
        public ICollection<AiPlayer> AiPlayers { get; set; }

        [ProtoMember(3)]
        public string MapName { get; set; }

        [ProtoMember(4)]
        public int FreeSlotsCount { get; set; }
    }
}
