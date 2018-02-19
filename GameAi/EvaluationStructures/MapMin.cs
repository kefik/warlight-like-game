namespace GameAi.EvaluationStructures
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using GameObjectsLib.GameMap;

    /// <summary>
    /// Represents minified version of the <see cref="Map"/>.
    /// </summary>
    internal struct MapMin
    {
        public RegionMin[] RegionsMin { get; }
        public SuperRegionMin[] SuperRegionsMin { get; }

        internal MapMin(RegionMin[] regionsMin, SuperRegionMin[] superRegionsMin)
        {
            RegionsMin = regionsMin;
            SuperRegionsMin = superRegionsMin;
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

        /// <summary>
        /// Performs deep copy of the MapMin structure.
        /// </summary>
        /// <returns></returns>
        internal MapMin DeepCopy()
        {
            RegionMin[] regionsMin = new RegionMin[RegionsMin.Length];

            for (int i = 0; i < RegionsMin.Length; i++)
            {
                var region = RegionsMin[i];

                var newRegion = new RegionMin(region.Id, region.SuperRegionId, region.Army, region.IsWasteland);

                int[] neighbourIds = new int[region.NeighbourRegionsIds.Length];
                Array.Copy(region.NeighbourRegionsIds, neighbourIds, neighbourIds.Length);

                newRegion.NeighbourRegionsIds = neighbourIds;

                regionsMin[i] = newRegion;
            }

            SuperRegionMin[] superRegionsMin = new SuperRegionMin[SuperRegionsMin.Length];

            for (int i = 0; i < SuperRegionsMin.Length; i++)
            {
                var superRegion = SuperRegionsMin[i];

                var newSuperRegion = new SuperRegionMin(superRegion.Id, superRegion.Bonus);

                int[] regionsIds = new int[superRegion.RegionsIds.Length];
                Array.Copy(superRegion.RegionsIds, regionsIds, regionsIds.Length);

                newSuperRegion.RegionsIds = regionsIds;
                newSuperRegion.PlayerEncoded = superRegion.PlayerEncoded;

                superRegionsMin[i] = newSuperRegion;
            }

            return new MapMin(regionsMin, superRegionsMin);
        }
    }
}