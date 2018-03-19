namespace GameAi.Data.EvaluationStructures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

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
        public byte PlayerId { get; set; }
        
        public PlayerPerspective(RegionMin[] regionsMin, SuperRegionMin[] superRegionsMin, byte playerId)
        {
            MapMin = new MapMin(regionsMin, superRegionsMin);
            PlayerId = playerId;
        }

        public PlayerPerspective(MapMin mapMin, byte playerId)
        {
            MapMin = mapMin;
            PlayerId = playerId;
        }

        /// <summary>
        /// Shallow-copies players perspective structure.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PlayerPerspective ShallowCopy()
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
                if (regionMin.OwnerId == PlayerId)
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

            // TODO: remove hardcoded string
            int basicIncome = 5;
            foreach (SuperRegionMin superRegionMin in superRegions)
            {
                if (IsSuperRegionMine(superRegionMin.Id))
                {
                    basicIncome += superRegionMin.Bonus;
                }
            }

            return basicIncome;
        }

        /// <summary>
        /// Obtains region by specified Id.
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public ref RegionMin GetRegion(int regionId)
        {
            return ref MapMin.GetRegion(regionId);
        }

        /// <summary>
        /// Returns <see cref="SuperRegionMin"/> based on its ID.
        /// </summary>
        /// <param name="superRegionId"></param>
        /// <returns></returns>
        public ref SuperRegionMin GetSuperRegion(int superRegionId)
        {
            return ref MapMin.GetSuperRegion(superRegionId);
        }

        public IEnumerable<RegionMin> GetNeighbourRegions(int regionId)
        {
            foreach (var neighbourRegionIds in GetRegion(regionId).NeighbourRegionsIds)
            {
                yield return GetRegion(neighbourRegionIds);
            }
        }
        
        public RegionMin GetClosestRegion(
            RegionMin sourceRegion,
            Func<RegionMin, bool> condition)
        {
            // BFS search for closest Region that is not mine
            // used region == key + parent region == value
            var usedRegions = new Dictionary<int, int>();
            
            Queue<RegionMin> nodesToProcess = new Queue<RegionMin>();
            nodesToProcess.Enqueue(sourceRegion);
            usedRegions.Add(sourceRegion.Id, sourceRegion.Id);
            do
            {
                var nodeToProcess = nodesToProcess.Dequeue();

                var neighbourRegions = GetNeighbourRegions(nodeToProcess.Id)
                    .Where(x => !usedRegions.ContainsKey(x.Id));
                foreach (RegionMin neighbourRegion in neighbourRegions)
                {
                    if (condition(neighbourRegion))
                    {
                        return neighbourRegion;
                    }

                    nodesToProcess.Enqueue(neighbourRegion);
                    usedRegions.Add(neighbourRegion.Id, nodeToProcess.Id);
                }
            } while (true);
        }

        /// <summary>
        /// True, if the player specified by 
        /// <see cref="PlayerId"/> has lost.
        /// </summary>
        /// <returns></returns>
        public bool HasLost()
        {
            return !GetMyRegions().Any();
        }
    }
}