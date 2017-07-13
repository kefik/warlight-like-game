﻿using System.Collections.Generic;

namespace ConquestObjectsLib.GameMap
{
    /// <summary>
    /// Represents type of the map, one for each map created for the game.
    /// </summary>
    enum MapType
    {
        None, World
    }

    /// <summary>
    /// Instance of this class represents map of the game.
    /// </summary>
    abstract class Map
    {
        public string Name { get; }
        /// <summary>
        /// Represents regions of the map that player can conquer.
        /// </summary>
        public ICollection<Region> Regions { get; } = new HashSet<Region>();
        
        /// <summary>
        /// Represents region groups this map has.
        /// </summary>
        public ICollection<SuperRegion> RegionGroups { get; } = new HashSet<SuperRegion>();
        
        protected Map(string name)
        {
            this.Name = name;
        }
    }
}