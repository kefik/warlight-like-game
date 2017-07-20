using System;
using System.Collections.Generic;
using GameObjectsLib.Game;
using GameObjectsLib;
using GameObjectsLib.GameMap;

namespace GameObjectsLib.Game
{
   class HotseatGame : Game
    {
        public override GameType GameType
        {
            get { return GameType.MultiplayerHotseat; }
        }

        public HotseatGame(Map map, ICollection<Player> players) : base(map, players)
        {
        }

        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }

}
