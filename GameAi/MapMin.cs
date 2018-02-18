namespace GameAi
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using GameObjectsLib.GameMap;

    /// <summary>
    /// Represents minified version of the <see cref="Map"/>.
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