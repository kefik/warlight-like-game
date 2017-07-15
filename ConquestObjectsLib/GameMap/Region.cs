using System.Collections.Generic;

namespace ConquestObjectsLib.GameMap
{
    /// <summary>
    /// Instace of this class represents region for given map in the game.
    /// </summary>
    public class Region
    {
        public string Name { get; }
        /// <summary>
        /// Player owning given region. Null means no owner.
        /// </summary>
        public Player Owner { get; set; }
        /// <summary>
        /// Number of units occuppying this region.
        /// </summary>
        public int Units { get; set; }
        /// <summary>
        /// Represents region group it belongs to.
        /// </summary>
        public SuperRegion SuperRegion { get; }

        /// <summary>
        /// Represents list of regions that are neighbours to this given region.
        /// </summary>
        public ICollection<Region> NeighbourRegions { get; } = new List<Region>();
        

        public Region(string name, int units, SuperRegion superRegion)
        {
            Name = name;
            Units = units;
            SuperRegion = superRegion;
        }

        
    }
}
