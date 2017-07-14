using System.Collections.Generic;

namespace ConquestObjectsLib.GameMap.Templates
{
    public class World : Map
    {
        public override int PlayerLimit { get; } = 10; // TODO: test for real max number of players

        /// <summary>
        /// Constructor initializing map named "World".
        /// </summary>
        public World() : base(nameof(World))
        {
            // TODO: Add initialization of Regions and SuperRegions
        }

        
    }
}
