﻿using System;
using System.Collections.Generic;
using GameObjectsLib.Game;
using GameObjectsLib;
using GameObjectsLib.GameMap;

namespace GameObjectsLib.Game
{
    class NetworkGame : Game
    {
        public override GameType GameType
        {
            get { return GameType.MultiplayerNetwork; }
        }

        public NetworkGame(int id, Map map, IList<Player> players) : base(id, map, players)
        {
        }
        
        /// <summary>
        /// Starts the game.
        /// </summary>
        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
