namespace GameAi
{
    using System;
    using System.Runtime.CompilerServices;
    using GameObjectsLib.GameMap;

    /// <summary>
    /// Represents bot from perspective of certain player.
    /// </summary>
    public struct PlayerPerspective
    {
        public Map Map;
        public byte PlayerEncoded { get; internal set; }
        
        public PlayerPerspective(RegionMin[] regionsMin, SuperRegionMin[] superRegionsMin, byte playerEncoded)
        {
            Map = new Map()
            {
                RegionsMin = regionsMin,
                SuperRegionsMin = superRegionsMin
            };
            PlayerEncoded = playerEncoded;
        }

        public PlayerPerspective(Map map, byte playerEncoded)
        {
            Map = map;
            PlayerEncoded = playerEncoded;
        }

        /// <summary>
        /// Shallow-copies players perspective structure.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PlayerPerspective ShallowCopy()
        {
            RegionMin[] regionsMin = new RegionMin[Map.RegionsMin.Length];
            Array.Copy(Map.RegionsMin, regionsMin, Map.RegionsMin.Length);

            SuperRegionMin[] superRegionsMin = new SuperRegionMin[Map.SuperRegionsMin.Length];
            Array.Copy(Map.SuperRegionsMin, superRegionsMin, Map.SuperRegionsMin.Length);

            return new PlayerPerspective
            {
                PlayerEncoded = PlayerEncoded,
                Map = new Map()
                {
                    RegionsMin = regionsMin,
                    SuperRegionsMin = superRegionsMin
                }
            };
        }
    }
}