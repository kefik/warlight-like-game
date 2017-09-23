namespace GameObjectsLib.NetworkCommObjects.Message
{
    using ProtoBuf;

    /// <summary>
    ///     Represents response from the server on create game attempt.
    /// </summary>
    [ProtoContract]
    public class CreateGameResponseMessage
    {
        /// <summary>
        ///     Notes whether the game was successfully created or not.
        /// </summary>
        [ProtoMember(1)]
        public bool Successful { get; set; }
    }
}
