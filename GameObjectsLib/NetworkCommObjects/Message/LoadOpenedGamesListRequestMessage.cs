namespace GameObjectsLib.NetworkCommObjects.Message
{
    using GameUser;
    using ProtoBuf;

    [ProtoContract]
    public class LoadOpenedGamesListRequestMessage
    {
        [ProtoMember(1)]
        public MyNetworkUser RequestingUser { get; set; }
    }
}
