namespace GameAi
{
    using System;
    using System.Linq;
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
        internal readonly MapMin MapMin;

        /// <summary>
        /// Represents encoded identification of the player.
        /// </summary>
        public byte PlayerEncoded { get; internal set; }
        
        internal PlayerPerspective(RegionMin[] regionsMin, SuperRegionMin[] superRegionsMin, byte playerEncoded)
        {
            MapMin = new MapMin(regionsMin, superRegionsMin);
            PlayerEncoded = playerEncoded;
        }

        internal PlayerPerspective(MapMin mapMin, byte playerEncoded)
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

        /// <summary>
        /// Returns true, if <see cref="regionMin"/> is my region.
        /// </summary>
        /// <param name="regionMin"></param>
        /// <returns></returns>
        public bool IsRegionMine(RegionMin regionMin)
        {
            return regionMin.GetOwnerPerspective(PlayerEncoded) == OwnerPerspective.Mine;
        }

        public bool IsRegionMine(int regionId)
        {
            RegionMin regionMin = MapMin.RegionsMin[regionId];

            return IsRegionMine(regionMin);
        }

        /// <summary>
        /// Finds out whether <see cref="regionMin"/> is neighbour
        /// to any region of <seealso cref="PlayerEncoded"/>.
        /// </summary>
        /// <param name="regionMin"></param>
        /// <returns></returns>
        public bool IsNeighbourToMyRegion(RegionMin regionMin)
        {
            // TODO: slow, refactor if I need it to invoke regularly
            return regionMin.NeighbourRegionsIds.Any(IsRegionMine);
        }
    }
}