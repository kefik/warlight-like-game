using System.Collections.Generic;
using ConquestObjectsLib.Game;
using ConquestObjectsLib.GameMap;

namespace ConquestObjectsLib.Game
{
    public class NetworkGame : Game
    {
        public NetworkGame(Map map, ICollection<Player> players) : base(map, players)
        {
        }

        public override GameType GameType
        {
            get { return GameType.MultiplayerNetwork; }
        }
    }

}
