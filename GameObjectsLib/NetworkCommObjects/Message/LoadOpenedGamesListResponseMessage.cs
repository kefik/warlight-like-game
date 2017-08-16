
namespace GameObjectsLib.NetworkCommObjects.Message
{
    using System.Collections.Generic;
    using ProtoBuf;

    [ProtoContract]
    public class LoadOpenedGamesListResponseMessage
    {
        [ProtoMember(1)]
        public IEnumerable<OpenedGameHeaderMessageObject> GameHeaderMessageObjects { get; set; }
    }
}
