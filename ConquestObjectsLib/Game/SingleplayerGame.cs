using System.Collections.Generic;
using ConquestObjectsLib.Game;
using ConquestObjectsLib.GameMap;

namespace ConquestObjectsLib.Game
{
    public class SingleplayerGame : Game
    {
        public SingleplayerGame(Map map, ICollection<Player> players) : base(map, players)
        {
        }

        public override GameType GameType
        {
            get
            {
                return GameType.SinglePlayer;
            }
        }
    }

}
