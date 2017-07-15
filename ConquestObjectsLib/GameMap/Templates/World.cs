using System;
using System.Collections.Generic;

namespace ConquestObjectsLib.GameMap.Templates
{
    /// <summary>
    /// Map representing the world map.
    /// </summary>
    class World : Map
    {
        /// <summary>
        /// Specifies upper limit for players for this map.
        /// </summary>
        public const int LimitPlayers = 10; // TODO: test for real max number of players

        public override MapType MapType
        {
            get { return MapType.World; }
        }

        public override int PlayersLimit
        {
            get { return LimitPlayers; }
        }

        /// <summary>
        /// Constructor initializing map named "World".
        /// </summary>
        public World() : base(nameof(World))
        {
            
        }

        public override void Initialize()
        {
            if (IsInitialized) throw new Exception(); // TODO: better exception

            IsInitialized = true;
            // TODO: Add initialization of Regions and SuperRegions
        }


    }
}
