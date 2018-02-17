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
        /// <summary>
        /// Minified version of the map.
        /// </summary>
        public readonly MapMin MapMin;

        /// <summary>
        /// Represents encoded identification of the player.
        /// </summary>
        public byte PlayerEncoded { get; }
        
        public PlayerPerspective(RegionMin[] regionsMin, SuperRegionMin[] superRegionsMin, byte playerEncoded)
        {
            MapMin = new MapMin(regionsMin, superRegionsMin);
            PlayerEncoded = playerEncoded;
        }

        public PlayerPerspective(MapMin mapMin, byte playerEncoded)
        {
            MapMin = mapMin;
            PlayerEncoded = playerEncoded;
        }

        /// <summary>
        /// Shallow-copies players perspective structure.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PlayerPerspective ShallowCopy()
        {
            return new PlayerPerspective(MapMin.ShallowCopy(), PlayerEncoded);
        }
    }
}