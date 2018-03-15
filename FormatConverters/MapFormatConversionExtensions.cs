﻿namespace FormatConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameAi.Data;
    using GameAi.Data.EvaluationStructures;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.Players;

    /// <summary>
    /// Handles conversion concerning the <see cref="Map"/>.
    /// </summary>
    public static class MapFormatConversionExtensions
    {
        /// <summary>
        /// Converts from <see cref="Map"/> format
        /// to <seealso cref="MapMin"/> minified format.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="playerIdsesMapper">
        /// Collection mapping IDs of players to IDs of bot players
        /// and the other way around.
        /// </param>
        /// <returns></returns>
        public static MapMin ToMapMin(this Map map,
            IIdsMapper playerIdsesMapper)
        {
            // add empty value

            // setup super regions
            var superRegions = map.SuperRegions
                .Select(x =>
                {
                    if (x.Owner == null)
                    {
                        return new SuperRegionMin(x.Id, x.Bonus);
                    }
                    if (playerIdsesMapper.TryGetNewId(x.Owner.Id, out int ownerId))
                    {
                        return new SuperRegionMin(x.Id, x.Bonus,
                            (byte) ownerId);
                    }
                    throw new ArgumentException("Player not mapped");
                })
                .ToArray();

            // setup regions
            var regions = map.Regions
                .Select(x =>
                {
                    if (x.Owner == null)
                    {
                        return new RegionMin(x.Id, x.SuperRegion.Id, x.Army);
                    }
                    if (playerIdsesMapper.TryGetNewId(x.Owner.Id, out int ownerId))
                    {
                        return new RegionMin(x.Id, x.SuperRegion.Id, x.Army,
                            (byte) ownerId);
                    }
                    throw new ArgumentException("Player not mapped");
                }).ToArray();

            // setup neighbours to those regions
            for (int index = 0; index < regions.Length; index++)
            {
                var region = regions[index];
                // get original regions neighbours
                var originalNeighbours = map.Regions
                    .First(x => x.Id == region.Id).NeighbourRegions;

                region.NeighbourRegionsIds = originalNeighbours
                    .Select(x => x.Id).ToArray();

                regions[index] = region;
            }

            for (int index = 0; index < superRegions.Length; index++)
            {
                var superRegion = superRegions[index];

                // get original SuperRegion regions
                var originalRegionsIds = map.SuperRegions
                    .First(x => x.Id == superRegion.Id)
                    .Regions.Select(x => x.Id);

                superRegion.RegionsIds = originalRegionsIds.ToArray();

                superRegions[index] = superRegion;
            }

            var mapMin = new MapMin(regions, superRegions);

            return mapMin;
        }

        /// <summary>
        /// Converts from <see cref="MapMin"/> minified format
        /// to <seealso cref="Map"/> format.
        /// </summary>
        /// <param name="mapMin">Minified map.</param>
        /// <param name="players">List of players.</param>
        /// <returns>Map created from <see cref="MapMin"/> instance.</returns>
        public static Map ToMap(this MapMin mapMin,
            IList<Player> players)
        {
            SuperRegion[] superRegions = mapMin.SuperRegionsMin
                .Select(x => new SuperRegion(x.Id, null, x.Bonus))
                .ToArray();

            Region[] regions = mapMin.RegionsMin
                .Select(x => new Region(x.Id, null, superRegions.FirstOrDefault(y => y.Id == x.SuperRegionId))
                {
                    Army = x.Army
                })
                .ToArray();

            foreach (Region region in regions)
            {
                region.ChangeOwner(players.First(y => y.Id == mapMin.RegionsMin.First(x => x.Id == region.Id).OwnerId));
            }

            // add regions to super region
            foreach (SuperRegion superRegion in superRegions)
            {
                // for each region that belongs to the super region => add it
                foreach (Region region in mapMin.SuperRegionsMin.First(x => x.Id == superRegion.Id)
                    .RegionsIds.Select(x => regions.First(y => y.Id == x)))
                {
                    superRegion.Regions.Add(region);
                }
            }

            // add neighbours to regions
            foreach (Region region in regions)
            {
                var neighbours = mapMin.RegionsMin.First(x => x.Id == region.Id).NeighbourRegionsIds
                    .Select(x => regions.First(y => y.Id == x));

                foreach (Region neighbour in neighbours)
                {
                    region.NeighbourRegions.Add(neighbour);
                }
            }

            return new Map(0, null, regions, superRegions);
        }
    }
}