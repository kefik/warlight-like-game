namespace GameObjectsLib.NetworkCommObjects.Message
{
    using ProtoBuf;

    /// <summary>
    /// Response to the load game attempt.
    /// </summary>
    /// <typeparam name="T">Can be <see cref="bool"/>,
    /// <see cref="Round"/>, <see cref="GameBeginningRound"/> or
    /// <see cref="Game.Game"/></typeparam>
    [ProtoContract]
    public class LoadGameResponseMessage<T>
    {
        [ProtoMember(1)]
        public T ResponseObject { get; set; }
    }
}
