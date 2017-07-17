using System;
using System.Collections.Generic;

namespace ConquestObjectsLib.GameMap
{
    /// <summary>
    /// Instance of this class represents map of the game.
    /// </summary>
    public sealed class Map // TODO: rework to non-abstract so the map can be dynamically loaded, remove some properties, mb add visual representation in it
    {
        public string Name { get; }
        /// <summary>
        /// Returns maximum number of players for the given map.
        /// </summary>
        public int PlayersLimit { get; }
        /// <summary>
        /// Represents regions of the map that player can conquer.
        /// </summary>
        public ICollection<Region> Regions { get; } = new HashSet<Region>();
        /// <summary>
        /// Represents region groups this map has.
        /// </summary>
        public ICollection<SuperRegion> SuperRegions { get; } = new HashSet<SuperRegion>();
        
        private Map(string name, int playersLimit)
        {
            Name = name;
            PlayersLimit = playersLimit;
        }

        /// <summary>
        /// Creates instance of map, initializes it,
        /// loads all objects related to its given model,
        /// getting the map ready for the start of the game.
        /// </summary>
        public static Map Create(string name, string playersLimit, string templatePath)
        {
            throw new NotImplementedException();
        }

        
    }
}
