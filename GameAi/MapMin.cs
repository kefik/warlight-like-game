namespace GameAi
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents minified version of the map.
    /// </summary>
    public struct MapMin
    {
        public RegionMin[] RegionsMin { get; }
        public SuperRegionMin[] SuperRegionsMin { get; }

        public MapMin(RegionMin[] regionsMin, SuperRegionMin[] superRegionsMin)
        {
            RegionsMin = regionsMin;
            SuperRegionsMin = superRegionsMin;
        }

        /// <summary>
        /// Reconstruct graph of objects, reconnecting all same objects so its represented only by one instance.
        /// </summary>
        public void ReconstructGraph()
        {
            foreach (var superRegion in SuperRegionsMin)
            {
                foreach (var superRegionRegion in superRegion.Regions)
                {
                    superRegionRegion.SuperRegion = superRegion;

                    for (int i = 0; i < RegionsMin.Length; i++)
                    {
                        if (RegionsMin[i] == superRegionRegion)
                        {
                            RegionsMin[i] = superRegionRegion;
                        }
                    }
                }
            }

            foreach (var region in RegionsMin)
            {
                for (int i = 0; i < region.NeighbourRegions.Length; i++)
                {
                    var realRegion = RegionsMin.First(x => x.Id == region.NeighbourRegions[i].Id);
                    region.NeighbourRegions[i] = realRegion;
                }
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal MapMin ShallowCopy()
        {
            RegionMin[] regionsMin = new RegionMin[RegionsMin.Length];
            Array.Copy(RegionsMin, regionsMin, RegionsMin.Length);

            SuperRegionMin[] superRegionsMin = new SuperRegionMin[SuperRegionsMin.Length];
            Array.Copy(SuperRegionsMin, superRegionsMin, SuperRegionsMin.Length);

            return new MapMin(regionsMin, superRegionsMin);
        }
    }
}