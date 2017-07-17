using System;
using System.Collections.Generic;
using ConquestObjectsLib.Game;
using ConquestObjectsLib.GameMap;

namespace ConquestObjectsLib.Game
{
    public class HotseatGame : Game
    {
        public override GameType GameType
        {
            get { return GameType.MultiplayerHotseat; }
        }

        public HotseatGame(GameMap.Map map, ICollection<Player> players) : base(map, players)
        {
        }

        

        public override void Start()
        {
            // TODO: validation
            HasStarted = true;
        }
    }

}
