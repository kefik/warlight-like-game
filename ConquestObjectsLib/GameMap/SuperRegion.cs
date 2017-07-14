using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConquestObjectsLib.GameMap
{
    /// <summary>
    /// Represents region group of giving size giving bonus to player who owns it all.
    /// </summary>
    class SuperRegion : IEnumerable<Region>
    {
        /// <summary>
        /// Name given of the SuperRegion.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Bonus given by the SuperRegion taken by one player.
        /// </summary>
        public int Bonus { get; }

        public Player Owner { get; private set; } // TODO: finish refreshing of situation, use GetOwner() method

        readonly ICollection<Region> regions = new HashSet<Region>();
        /// <summary>
        /// Constructs SuperRegion instance.
        /// </summary>
        /// <param name="name">Name of the new SuperRegion.</param>
        /// <param name="bonus">Bonus given by SuperRegion.</param>
        public SuperRegion(string name, int bonus)
        {
            Name = name;
            Bonus = bonus;
        }
        /// <summary>
        /// Adds region to this SuperRegion collections.
        /// </summary>
        /// <param name="region">Region to be added.</param>
        /// <exception cref="ArgumentException">If region's SuperRegion is not same as this</exception>
        public void AddRegion(Region region)
        {
            if (this != region?.SuperRegion) throw new ArgumentException();

            regions.Add(region);
        }
        /// <summary>
        /// Removes region if it's contained.
        /// </summary>
        /// <param name="region">Region to be removed.</param>
        /// <returns>True if the region is successfully removed.</returns>
        public bool RemoveRegion(Region region)
        {
            return regions.Remove(region);
        }
        /// <summary>
        /// Finds out whether the region in collection contains region.
        /// </summary>
        /// <param name="region">Region to be removed.</param>
        /// <returns>True if collection of regions contains region specified in parameter.</returns>
        public bool ContainsRegion(Region region)
        {
            return regions.Contains(region);
        }

        /// <summary>
        /// Calculates owner of this SuperRegion.
        /// </summary>
        /// <returns>Owner of this SuperRegion, or null.</returns>
        private Player GetOwner()
        {
            var firstOwner = regions.FirstOrDefault()?.Owner;
            return (from region in regions where region.Owner != firstOwner select region).Any() // if there exists any such that hes not same as the first owner
                ? null // return null
                : firstOwner;
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
