namespace GameObjectsLib.NetworkCommObjects.Message
{
    using GameUser;
    using ProtoBuf;

    /// <summary>
    ///     Request to load a game specified by GameId.
    /// </summary>
    [ProtoContract]
    public class LoadGameRequestMessage
    {
        [ProtoMember(1)]
        public MyNetworkUser RequestingUser { get; set; }

        [ProtoMember(2)]
        public int GameId { get; set; }

        [ProtoMember(3)]
        public int RoundNumber { get; set; }
    }
}
