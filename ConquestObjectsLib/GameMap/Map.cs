using System.Collections.Generic;
using ConquestObjectsLib.GameMap.Templates;

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

        protected bool isInitialized;

        public string Name { get; }
        /// <summary>
        /// Returns maximum number of players for the given map.
        /// </summary>
        public abstract int PlayersLimit { get; }
        /// <summary>
        /// Represents regions of the map that player can conquer.
        /// </summary>
        public ICollection<Region> Regions { get; } = new HashSet<Region>();
        /// <summary>
        /// Determines what type the given map has.
        /// </summary>
        public abstract MapType MapType { get; }
        /// <summary>
        /// Represents region groups this map has.
        /// </summary>
        public ICollection<SuperRegion> SuperRegions { get; } = new HashSet<SuperRegion>();
        
        protected Map(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// Initializes the map, loading all objects related to its given model,
        /// getting the map ready for the start of the game.
        /// Can be called only once.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Factory method constructing map depending on the parameter.
        /// </summary>
        /// <param name="map">Decides what kind of template use to construct the map.</param>
        /// <returns>Map</returns>
        public static Map ConstructMap(MapType map)
        {
            switch (map)
            {
                case MapType.World:
                    return new World();
                default:
                    return null;
            }
        }
    }
}
