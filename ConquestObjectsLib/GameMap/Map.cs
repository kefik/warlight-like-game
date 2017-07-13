using System.Collections.Generic;

namespace ConquestObjectsLib.GameMap
{
    /// <summary>
    /// Represents type of the map, one for each map created for the game.
    /// </summary>
    enum MapType
    {
        World
    }
    abstract class Map
    {
        public string Name { get; }
        /// <summary>
        /// Represents regions of the map that player can conquer.
        /// </summary>
        public ICollection<Region> Regions { get; protected set; }

        protected Map(string name)
        {
            this.Name = name;
        }
    }
}
