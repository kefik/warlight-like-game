using System.Collections.Generic;

namespace ConquestObjectsLib.GameMap
{
    /// <summary>
    /// Represents type of the map, one for each map created for the game.
    /// </summary>
    public enum MapType
    {
        None, World
    }

    /// <summary>
    /// Instance of this class represents map of the game.
    /// </summary>
    public abstract class Map
    {
        public string Name { get; }
        /// <summary>
        /// Represents regions of the map that player can conquer.
        /// </summary>
        public ICollection<Region> Regions { get; } = new HashSet<Region>();
        
        /// <summary>
        /// Represents region groups this map has.
        /// </summary>
        public ICollection<SuperRegion> SuperRegions { get; } = new HashSet<SuperRegion>();

        /// <summary>
        /// Specifies upper limit for players for this map.
        /// </summary>
        public abstract int PlayerLimit { get; }

        protected Map(string name)
        {
            this.Name = name;
        }
    }
}
