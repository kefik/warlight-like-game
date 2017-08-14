using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace GameObjectsLib.NetworkCommObjects.Message
{
    public class GameHeaderMessageObject
    {
        public int GameId { get; set; }

        public string MapName { get; set; }
        
        public int AiPlayersCount { get; set; }
        
        public int HumanPlayersCount { get; set; }

        public DateTime GameStarted { get; set; }

        public DateTime RoundStarted { get; set; }

        public override string ToString()
        {
            return $"Human: {HumanPlayersCount}, Ai: {AiPlayersCount}, Map: {MapName}, Game-started: {GameStarted}";
        }
    }

    /// <summary>
    /// Gives response whether the request was valid.
    /// </summary>
    [ProtoContract]
    public class LoadMyGamesListResponseMessage
    {
        [ProtoMember(1)]
        public IEnumerable<GameHeaderMessageObject> GameHeaderMessageObjects { get; set; }
    }


}
