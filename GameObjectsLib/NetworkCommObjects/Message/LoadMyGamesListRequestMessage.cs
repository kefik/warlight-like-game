namespace GameObjectsLib.NetworkCommObjects.Message
{
    using GameUser;
    using ProtoBuf;

    /// <summary>
    ///     Loads list of games started by the specified <see cref="RequestingUser" />
    /// </summary>
    [ProtoContract]
    public class LoadMyGamesListRequestMessage
    {
        [ProtoMember(1)]
        public MyNetworkUser RequestingUser { get; set; }
    }
}
