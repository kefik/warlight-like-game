namespace GameObjectsLib.NetworkCommObjects.Message
{
    using GameUser;
    using ProtoBuf;

    [ProtoContract]
    public class LoadOpenedGameRequestMessage
    {
        [ProtoMember(1)]
        public MyNetworkUser MyUser { get; set; }
        [ProtoMember(2)]
        public int GameId { get; set; }
    }
}
