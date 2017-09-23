namespace GameObjectsLib.NetworkCommObjects.Message
{
    using ProtoBuf;

    [ProtoContract]
    [ProtoInclude(100, typeof(StartedGameHeaderMessageObject))]
    [ProtoInclude(101, typeof(OpenedGameHeaderMessageObject))]
    public abstract class GameHeaderMessageObject
    {
        [ProtoMember(1)]
        public int GameId { get; set; }

        [ProtoMember(2)]
        public string MapName { get; set; }

        [ProtoMember(3)]
        public int AiPlayersCount { get; set; }

        [ProtoMember(4)]
        public int HumanPlayersCount { get; set; }
    }
}