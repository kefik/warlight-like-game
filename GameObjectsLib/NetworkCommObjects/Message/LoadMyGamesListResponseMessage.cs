namespace GameObjectsLib.NetworkCommObjects.Message
{
    using System;
    using System.Collections.Generic;
    using ProtoBuf;

    [ProtoContract]
    [ProtoInclude(100, typeof(StartedGameHeaderMessageObject))]
    [ProtoInclude(101, typeof(OpenedGameHeaderMessageObject))]
    public abstract class GameHeaderMessageObject
    {
        [ProtoMember(1)]
        public int GameId { get; set; }

        [ProtoMember(2)]
        public string MapName { get; set; }

        [ProtoMember(3)]
        public int AiPlayersCount { get; set; }

        [ProtoMember(4)]
        public int HumanPlayersCount { get; set; }
    }

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
