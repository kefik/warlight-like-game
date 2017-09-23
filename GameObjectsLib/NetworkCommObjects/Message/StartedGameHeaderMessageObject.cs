namespace GameObjectsLib.NetworkCommObjects.Message
{
    using System;
    using ProtoBuf;

    [ProtoContract]
    public class StartedGameHeaderMessageObject : GameHeaderMessageObject
    {
        [ProtoMember(5)]
        public DateTime GameStarted { get; set; }

        [ProtoMember(6)]
        public DateTime RoundStarted { get; set; }

        public override string ToString()
        {
            return
                $"Human: {HumanPlayersCount}, Ai: {AiPlayersCount}, Map: {MapName}, Game-started: {GameStarted}, Round-started: {RoundStarted}";
        }
    }
}