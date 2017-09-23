namespace GameObjectsLib.NetworkCommObjects.Message
{
    using GameUser;
    using ProtoBuf;

    /// <summary>
    ///     Attempts to log in to the server.
    /// </summary>
    [ProtoContract]
    public class UserLogInRequestMessage
    {
        [ProtoMember(1)]
        public MyNetworkUser LoggingUser { get; set; }
    }
}
