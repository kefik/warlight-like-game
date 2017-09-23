namespace GameObjectsLib.NetworkCommObjects.Message
{
    using System.Collections.Generic;
    using ProtoBuf;


    /// <summary>
    ///     Gives response whether the request was valid.
    /// </summary>
    [ProtoContract]
    public class LoadMyGamesListResponseMessage
    {
        [ProtoMember(1)]
        public IEnumerable<GameHeaderMessageObject> GameHeaderMessageObjects { get; set; }
    }
}
