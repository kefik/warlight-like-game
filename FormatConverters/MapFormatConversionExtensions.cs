namespace FormatConverters
{
    using System;
    using System.Linq;
    using GameAi.Data.EvaluationStructures;
    using GameObjectsLib.GameMap;

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
        /// <returns></returns>
        public static MapMin ToMapMin(this Map map)
        {
            // setup super regions
            var superRegions = map.SuperRegions
                .Select(x => x.Owner == null ? new SuperRegionMin(x.Id, x.Bonus)
                : new SuperRegionMin(x.Id, x.Bonus, (byte)x.Owner.Id))
                .ToArray();

            // setup regions
            var regions = map.Regions
                .Select(x => x.Owner == null ?
                new RegionMin(x.Id, x.SuperRegion.Id, x.Army)
                : new RegionMin(x.Id, x.SuperRegion.Id, x.Army,
                (byte)x.Owner.Id)).ToArray();

            // setup neighbours to those regions
            for (int index = 0; index < regions.Length; index++)
            {
                var region = regions[index];
                // get original regions neighbours
                var originalNeighbours = map.Regions
                    .First(x => x.Id == region.Id).NeighbourRegions;

                region.NeighbourRegionsIds = originalNeighbours.Select(x => x.Id).ToArray();

                regions[index] = region;
            }

            for (int index = 0; index < superRegions.Length; index++)
            {
                var superRegion = superRegions[index];

                // get original SuperRegion regions
                var originalRegionsIds = map.SuperRegions.First(x => x.Id == superRegion.Id).Regions.Select(x => x.Id);

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
        /// <param name="mapMin"></param>
        /// <returns></returns>
        public static Map ToMap(this MapMin mapMin)
        {
            throw new NotImplementedException();
        }
    }
}