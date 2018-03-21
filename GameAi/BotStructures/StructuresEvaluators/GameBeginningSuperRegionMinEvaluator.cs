namespace GameAi.BotStructures.StructuresEvaluators
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Extensions;
    using Data.EvaluationStructures;
    using Interfaces;
    using Interfaces.Evaluators.StructureEvaluators;

    /// <summary>
    /// Component that can evaluate <see cref="SuperRegionMin"/>
    /// instance.
    /// </summary>
    internal sealed class GameBeginningSuperRegionMinEvaluator : CacheableEvaluator,
        ISuperRegionMinEvaluator
    {
        private const double SuperRegionsRegionsCountCoeficient = 5;
        private const double ForeignNeighboursCoefficient = 4;
        private const double BonusCoefficient = 1;

        public GameBeginningSuperRegionMinEvaluator(MapMin map)
        {
            InitializeCache(map);
            NormalizeToPositiveNumbers(StaticCache);
        }

        /// <summary>
        /// Obtains value describing SuperRegion value in
        /// specified game state.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <param name="gameStructure"></param>
        /// <returns></returns>
        public double GetValue(PlayerPerspective currentGameState,
            SuperRegionMin gameStructure)
        {
            double staticValue = GetStaticValue(gameStructure);
            // TODO: better calculation

            return staticValue;
        }

        private void NormalizeToPositiveNumbers(IDictionary<int, double> dictionary)
        {
            double minValue = dictionary.Min(x => x.Value);

            double numberToAdd = minValue < 0 ? -minValue : minValue;
            numberToAdd++;

            for (int i = 0; i < dictionary.Count; i++)
            {
                StaticCache[i] += numberToAdd;
            }
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

        protected override void InitializeCache(MapMin map)
        {
            foreach (SuperRegionMin gameStructure in map.SuperRegionsMin)
            {
                // hint: greater bonus => greater value
                double bonusValue = BonusCoefficient * gameStructure.Bonus;

                // regions of the super region
                var regionsIds = gameStructure
                    .RegionsIds
                    .Select(x => x)
                    .ToList();

                // foreign neighbours are neighbours that do not belong to the same super region
                var foreignNeighbourRegionsIds = (from regionId in regionsIds
                                                  from neighbourId in map.GetRegion(regionId)
                                                      .NeighbourRegionsIds
                                                  where map.GetRegion(neighbourId).SuperRegionId != gameStructure.Id
                                                  select neighbourId)
                                                  .ToList();

                // hint: more foreign neighbour it has, worse it is
                int foreignNeighboursCount = foreignNeighbourRegionsIds.Count;
                double foreignNeighboursValue = ForeignNeighboursCoefficient * foreignNeighboursCount;

                // hint: more super region neighbours, the better it is
                int neighbourSuperRegionsCount = foreignNeighbourRegionsIds
                    .Select(x => map.GetRegion(x).SuperRegionId)
                    .Distinct().Count();
                double neighbourSuperRegionsValue = neighbourSuperRegionsCount;

                // hint: more regions it has, worse it is (harder to conquer especially at the beginning)
                double regionsValue = SuperRegionsRegionsCountCoeficient * gameStructure.RegionsIds.Length;

                double staticResult = bonusValue + neighbourSuperRegionsValue
                                      - regionsValue - foreignNeighboursValue;

                StaticCache.Add(gameStructure.Id, staticResult);
            }
        }
    }
}