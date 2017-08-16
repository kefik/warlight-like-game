namespace GameObjectsLib.NetworkCommObjects.Message
{
    using ProtoBuf;

    [ProtoContract]
    public class LoadOpenedGameResponseMessage
    {
        [ProtoMember(1)]
        public byte[] SerializedGame { get; set; }
    }
}
