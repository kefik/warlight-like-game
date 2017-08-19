namespace GameObjectsLib.NetworkCommObjects.Message
{
    using ProtoBuf;
    using Game;

    [ProtoContract]
    public class LoadOpenedGameResponseMessage
    {
        [ProtoMember(1)]
        public Game Game { get; set; }
    }
}
