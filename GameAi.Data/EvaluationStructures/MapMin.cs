namespace GameAi.Data.EvaluationStructures
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents minified version of the Map.
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
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MapMin ShallowCopy()
        {
            RegionMin[] regionsMin = new RegionMin[RegionsMin.Length];
            Array.Copy(RegionsMin, regionsMin, RegionsMin.Length);

            SuperRegionMin[] superRegionsMin = new SuperRegionMin[SuperRegionsMin.Length];
            Array.Copy(SuperRegionsMin, superRegionsMin, SuperRegionsMin.Length);

            return new MapMin(regionsMin, superRegionsMin);
        }

        /// <summary>
        /// Performs deep copy of the MapMin structure.
        /// </summary>
        /// <returns></returns>
        public MapMin DeepCopy()
        {
            RegionMin[] regionsMin = new RegionMin[RegionsMin.Length];

            for (int i = 0; i < RegionsMin.Length; i++)
            {
                ref var region = ref RegionsMin[i];

                var newRegion = new RegionMin(region.Id, region.SuperRegionId, region.Army, region.OwnerId, region.IsWasteland)
                {
                    Name = region.Name
                };

                int[] neighbourIds = new int[region.NeighbourRegionsIds.Length];
                Array.Copy(region.NeighbourRegionsIds, neighbourIds, neighbourIds.Length);

                newRegion.NeighbourRegionsIds = neighbourIds;

                regionsMin[i] = newRegion;
            }

            SuperRegionMin[] superRegionsMin = new SuperRegionMin[SuperRegionsMin.Length];

            for (int i = 0; i < SuperRegionsMin.Length; i++)
            {
                ref var superRegion = ref SuperRegionsMin[i];

                var newSuperRegion = new SuperRegionMin(superRegion.Id, superRegion.Bonus, superRegion.OwnerId)
                {
                    Name = superRegion.Name
                };

                int[] regionsIds = new int[superRegion.RegionsIds.Length];
                Array.Copy(superRegion.RegionsIds, regionsIds, regionsIds.Length);

                newSuperRegion.RegionsIds = regionsIds;

                superRegionsMin[i] = newSuperRegion;
            }

            return new MapMin(regionsMin, superRegionsMin);
        }

        /// <summary>
        /// Reports whether the current situation is the game beginning.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Slow, don't use frequently.</remarks>
        [Pure]
        public bool IsGameBeginning()
        {
            return RegionsMin.All(x => x.OwnerId == 0);
        }

        /// <summary>
        /// Obtains region by specified Id.
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public ref RegionMin GetRegion(int regionId)
        {
            return ref RegionsMin[regionId];
        }

        /// <summary>
        /// Returns <see cref="SuperRegionMin"/> based on its ID.
        /// </summary>
        /// <param name="superRegionId"></param>
        /// <returns></returns>
        public ref SuperRegionMin GetSuperRegion(int superRegionId)
        {
            return ref SuperRegionsMin[superRegionId];
        }

        /// <summary>
        /// Returns list of neighbour regions of given <see cref="RegionMin"/>.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public IEnumerable<RegionMin> GetNeighbourRegions(RegionMin region)
        {
            foreach (int regionNeighbourRegionsId in region.NeighbourRegionsIds)
            {
                yield return GetRegion(regionNeighbourRegionsId);
            }
        }
    }
}