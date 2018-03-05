namespace GameAi.EvaluationStructures
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.Players;

    /// <summary>
    /// Represents bot from perspective of certain player.
    /// </summary>
    internal struct PlayerPerspective
    {
        /// <summary>
        /// Minified version of the map.
        /// </summary>
        internal readonly MapMin MapMin;

        /// <summary>
        /// Represents encoded identification of the player.
        /// </summary>
        public byte PlayerId { get; internal set; }
        
        internal PlayerPerspective(RegionMin[] regionsMin, SuperRegionMin[] superRegionsMin, byte playerId)
        {
            MapMin = new MapMin(regionsMin, superRegionsMin);
            PlayerId = playerId;
        }

        internal PlayerPerspective(MapMin mapMin, byte playerId)
        {
            MapMin = mapMin;
            PlayerId = playerId;
        }

        /// <summary>
        /// Shallow-copies players perspective structure.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PlayerPerspective ShallowCopy()
        {
            return new PlayerPerspective(MapMin.ShallowCopy(), PlayerId);
        }

        /// <summary>
        /// Returns true, if <see cref="regionMin"/> is my region.
        /// </summary>
        /// <param name="regionMin"></param>
        /// <returns></returns>
        public bool IsRegionMine(RegionMin regionMin)
        {
            return regionMin.GetOwnerPerspective(PlayerId) == OwnerPerspective.Mine;
        }

        public bool IsRegionMine(int regionId)
        {
            RegionMin regionMin = MapMin.RegionsMin[regionId];

            return IsRegionMine(regionMin);
        }

        public bool IsSuperRegionMine(int superRegionId)
        {
            return MapMin.SuperRegionsMin[superRegionId].OwnerId == PlayerId;
        }

        /// <summary>
        /// Finds out whether <see cref="regionMin"/> is neighbour
        /// to any region of <seealso cref="PlayerId"/>.
        /// </summary>
        /// <param name="regionMin"></param>
        /// <returns></returns>
        public bool IsNeighbourToAnyMyRegion(RegionMin regionMin)
        {
            foreach (int regionsId in regionMin.NeighbourRegionsIds)
            {
                if (IsRegionMine(regionsId))
                {
                    return true;
                }
            }

            return false;
        }
        
        public IEnumerable<RegionMin> GetMyRegions()
        {
            foreach (RegionMin regionMin in MapMin.RegionsMin)
            {
                if (regionMin.GetOwnerPerspective(PlayerId) == OwnerPerspective.Mine)
                {
                    yield return regionMin;
                }
            }
        }
        
        /// <summary>
        /// Obtains income of player at the current state of the game.
        /// </summary>
        /// <returns></returns>
        public int GetMyIncome()
        {
            var superRegions = MapMin.SuperRegionsMin;

            int basicIncome = Player.BasicIncome;
            foreach (SuperRegionMin superRegionMin in superRegions)
            {
                if (IsSuperRegionMine(superRegionMin.Id))
                {
                    basicIncome += superRegionMin.Bonus;
                }
            }

            return basicIncome;
        }
    }
}