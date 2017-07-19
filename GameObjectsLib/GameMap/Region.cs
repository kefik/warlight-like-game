using System.Collections.Generic;
using GameObjectsLib;
using GameObjectsLib.GameMap;

namespace GameObjectsLib.GameMap
{
    /// <summary>
    /// Instace of this class represents region for given map in the game.
    /// </summary>
    public class Region
    {
        public int Id { get; }
        public string Name { get; }
        /// <summary>
        /// Player owning given region. Null means no owner.
        /// </summary>
        public Player Owner { get; set; }

        /// <summary>
        /// Number of units occuppying this region.
        /// </summary>
        public int Army { get; set; }
        /// <summary>
        /// Represents region group it belongs to.
        /// </summary>
        public SuperRegion SuperRegion { get; }

        /// <summary>
        /// Represents list of regions that are neighbours to this given region.
        /// </summary>
        public ICollection<Region> NeighbourRegions { get; } = new List<Region>();
        

        public Region(int id, string name, SuperRegion superRegion)
        {
            Id = id;
            Name = name;
            SuperRegion = superRegion;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
