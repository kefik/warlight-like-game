namespace GameAi.BotStructures.StructuresEvaluators
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;
    using Interfaces.Evaluators.StructureEvaluators;

    internal sealed class GameSuperRegionMinEvaluator : CacheableEvaluator,
        ISuperRegionMinEvaluator
    {
        private const double SuperRegionsRegionsCountCoeficient = 5;
        private const double ForeignNeighboursCoefficient = 4;
        private const double BonusCoefficient = 1;

        public GameSuperRegionMinEvaluator(MapMin map)
        {
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
                double bonusValue = BonusCoefficient * gameStructure.Bonus;

                var regions = gameStructure
                    .RegionsIds
                    .Select(x => map.GetRegion(x))
                    .ToList();

                var foreignNeighbourRegionsIds = (from region in regions
                                                  from neighbourId in region
                                                      .NeighbourRegionsIds
                                                  where region.Id != neighbourId
                                                  select neighbourId).ToList();

                // hint: more foreign neighbour it has, worse it is
                int foreignNeighboursCount = foreignNeighbourRegionsIds.Count;
                double foreignNeighbourValue = ForeignNeighboursCoefficient * foreignNeighboursCount;

                // hint: more super region neighbours, the better it is
                int neighbourSuperRegionsCount = foreignNeighbourRegionsIds
                    .Select(x => map.GetRegion(x).SuperRegionId)
                    .Distinct().Count();
                double neighbourSuperRegionsValue = neighbourSuperRegionsCount;

                // hint: more regions it has, worse it is (harder to conquer especially at the beginning)
                double regionsValue = SuperRegionsRegionsCountCoeficient
                                      * gameStructure.RegionsIds.Length;

                double staticResult = bonusValue
                                      - neighbourSuperRegionsValue
                                      - regionsValue
                                      - foreignNeighbourValue;

                StaticCache.Add(gameStructure.Id, staticResult);
            }
        }

        public double GetValue(PlayerPerspective currentGameState, SuperRegionMin gameStructure)
        {
            double staticValue = GetStaticValue(gameStructure);

            return staticValue;
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