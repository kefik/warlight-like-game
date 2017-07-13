using System;
using System.Collections;
using System.Collections.Generic;

namespace ConquestObjectsLib.GameMap
{
    /// <summary>
    /// Represents region group of giving size giving bonus to player who owns it all.
    /// </summary>
    class RegionGroup : IEnumerable<Region>
    {
        public string Name { get; }
        public int Bonus { get; }
        readonly ICollection<Region> regions = new HashSet<Region>();

        public RegionGroup(string name, int bonus)
        {
            Name = name;
            Bonus = bonus;
        }

        public void AddRegion(Region region)
        {
            if (this != region?.RegionGroup) throw new ArgumentException();

            regions.Add(region);
        }

        public bool RemoveRegion(Region region)
        {
            return regions.Remove(region);
        }

        public bool ContainsRegion(Region region)
        {
            return regions.Contains(region);
        }


        public IEnumerator<Region> GetEnumerator()
        {
            foreach (Region region in regions)
            {
                yield return region;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
