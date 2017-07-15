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
            get { return GameType.MultiplayerNetwork; }
        }

        public HotseatGame(Map map, ICollection<Player> players) : base(map, players)
        {
        }

        

        public override void Start()
        {
            Map.Initialize();
            // TODO: validation
            HasStarted = true;
        }
    }

}
