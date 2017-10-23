namespace GameAi
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents bot from perspective of certain player.
    /// </summary>
    public struct PlayerPerspective
    {
        public RegionMin[] RegionsMin { get; internal set; }
        public SuperRegionMin[] SuperRegionsMin { get; internal set; }
        public byte PlayerEncoded { get; internal set; }

        //internal PlayerPerspective() { }

        /// <summary>
        /// Shallow-copies players perspective structure.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PlayerPerspective ShallowCopy()
        {
            RegionMin[] regionsMin = new RegionMin[RegionsMin.Length];
            Array.Copy(RegionsMin, regionsMin, RegionsMin.Length);

            SuperRegionMin[] superRegionsMin = new SuperRegionMin[SuperRegionsMin.Length];
            Array.Copy(SuperRegionsMin, superRegionsMin, SuperRegionsMin.Length);

            return new PlayerPerspective
            {
                PlayerEncoded = PlayerEncoded,
                RegionsMin = regionsMin,
                SuperRegionsMin = superRegionsMin
            };
        }
    }
}