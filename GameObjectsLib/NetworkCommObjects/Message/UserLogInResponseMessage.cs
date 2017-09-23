namespace GameObjectsLib.NetworkCommObjects.Message
{
    using ProtoBuf;

    /// <summary>
    ///     Response whether the attempt to log in was successful.
    /// </summary>
    [ProtoContract]
    public class UserLogInResponseMessage
    {
        [ProtoMember(1)]
        public bool SuccessfullyLoggedIn { get; set; }

        [ProtoMember(2)]
        public string Email { get; set; }
    }
}
