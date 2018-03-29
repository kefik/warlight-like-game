namespace GameAi.BotStructures.StructuresEvaluators
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;
    using Interfaces.Evaluators.StructureEvaluators;

    internal sealed class GameSuperRegionMinEvaluator : CacheableEvaluator,
        ISuperRegionMinEvaluator
    {
        private readonly double superRegionsRegionsCountCoeficient;
        private readonly double foreignNeighboursCoefficient;
        private readonly double bonusCoefficient;
        private readonly double conqueredCoefficient;

        public GameSuperRegionMinEvaluator(MapMin map,
            double bonusCoefficient,
            double conqueredCoefficient,
            double foreignNeighboursCoefficient,
            double superRegionsRegionsCountCoefficient)
        {
            this.superRegionsRegionsCountCoeficient = superRegionsRegionsCountCoefficient;
            this.foreignNeighboursCoefficient = foreignNeighboursCoefficient;
            this.bonusCoefficient = bonusCoefficient;
            this.conqueredCoefficient = conqueredCoefficient;
            // nothing added so far
            InitializeCache(map);
            // find out the minimum value
            double min = map.SuperRegionsMin
                .Min(x => GetStaticValue(x));
            // value that should be added to every dictionary entry
            double valueToAdd = min < 0 ? -min : min;
            valueToAdd++;

            // add value for every entry in the dictionary
            for (int i = 0; i < map.SuperRegionsMin.Length; i++)
            {
                StaticCache[i] += valueToAdd;
            }
        }

        protected override void InitializeCache(MapMin map)
        {
            foreach (SuperRegionMin gameStructure in map.SuperRegionsMin)
            {
                // hint: greater bonus => lesser cost
                double bonusValue = bonusCoefficient * gameStructure.Bonus;

                var regions = gameStructure
                    .RegionsIds
                    .Select(x => map.GetRegion(x))
                    .ToList();

                // foreign neighbours are neighbours that do not belong to the same super region
                var foreignNeighbourRegionsIds = (from region in regions
                                                  from neighbourId in map.GetRegion(region.Id)
                                                      .NeighbourRegionsIds
                                                  where map.GetRegion(neighbourId).SuperRegionId != gameStructure.Id
                                                  select neighbourId)
                                                .ToList();

                // hint: more foreign neighbour it has, worse it is
                int foreignNeighboursCount = foreignNeighbourRegionsIds.Count;
                double foreignNeighbourValue = foreignNeighboursCoefficient * foreignNeighboursCount;

                // hint: more super region neighbours, the better it is
                int neighbourSuperRegionsCount = foreignNeighbourRegionsIds
                    .Select(x => map.GetRegion(x).SuperRegionId)
                    .Distinct().Count();
                double neighbourSuperRegionsValue = neighbourSuperRegionsCount;

                // hint: more regions it has, worse it is (harder to conquer especially at the beginning)
                double regionsValue = superRegionsRegionsCountCoeficient
                                      * gameStructure.RegionsIds.Length;

                double staticResult = bonusValue
                                      - neighbourSuperRegionsValue
                                      - regionsValue
                                      - foreignNeighbourValue;

                StaticCache.Add(gameStructure.Id, staticResult);
            }
        }

        public double GetValue(PlayerPerspective currentGameState,
            SuperRegionMin gameStructure)
        {
            double staticValue = GetStaticValue(gameStructure);

            double dynamicValue = 0;
            if (gameStructure.OwnerId != 0)
            {
                dynamicValue = conqueredCoefficient * (gameStructure.Bonus * gameStructure.Bonus);
            }

            return staticValue + dynamicValue;
        }

        /// <summary>
        /// Calculates value of given super region
        /// taking only non-changing (static)
        /// fields into account.
        /// </summary>
        /// <param name="gameStructure"></param>
        /// <returns></returns>
        private double GetStaticValue(SuperRegionMin gameStructure)
        {
            return StaticCache[gameStructure.Id];
        }
    }
}