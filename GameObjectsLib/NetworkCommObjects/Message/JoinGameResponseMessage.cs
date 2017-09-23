namespace GameObjectsLib.NetworkCommObjects.Message
{
    using ProtoBuf;

    /// <summary>
    ///     Binary response to the join game attempt.
    /// </summary>
    [ProtoContract]
    public class JoinGameResponseMessage
    {
        [ProtoMember(1)]
        public bool SuccessfullyJoined { get; set; }
    }
}
