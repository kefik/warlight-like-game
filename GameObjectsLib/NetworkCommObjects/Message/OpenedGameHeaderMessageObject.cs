namespace GameObjectsLib.NetworkCommObjects.Message
{
    using System;
    using ProtoBuf;

    [ProtoContract]
    public class OpenedGameHeaderMessageObject : GameHeaderMessageObject
    {
        [ProtoMember(7)]
        public DateTime GameCreated { get; set; }

        public override string ToString()
        {
            return $"Human: {HumanPlayersCount}, Ai: {AiPlayersCount}, Map: {MapName}, Game-created: {GameCreated}";
        }
    }
}