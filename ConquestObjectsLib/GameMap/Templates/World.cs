using System.Collections.Generic;

namespace ConquestObjectsLib.GameMap.Templates
{
    /// <summary>
    /// Map representing the world map.
    /// </summary>
    public class World : Map
    {
        /// <summary>
        /// Specifies upper limit for players for this map.
        /// </summary>
        public const int PlayersLimit = 10; // TODO: test for real max number of players

        public override int PlayersMax
        {
            get { return PlayersLimit; }
        }

        /// <summary>
        /// Constructor initializing map named "World".
        /// </summary>
        public World() : base(nameof(World))
        {
            // TODO: Add initialization of Regions and SuperRegions
        }

        
    }
}
