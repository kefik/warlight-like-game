namespace GameAi.BotStructures.StructuresEvaluators
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;
    using Interfaces;
    using Interfaces.Evaluators.StructureEvaluators;

    /// <summary>
    /// Component that can evaluate <see cref="SuperRegionMin"/>
    /// instance.
    /// </summary>
    internal class GameBeginningSuperRegionMinEvaluator : ISuperRegionMinEvaluator
    {
        private const double SuperRegionsRegionsCountCoeficient = 5;
        private const double ForeignNeighboursCoefficient = 4;
        private const double BonusCoefficient = 1;

        /// <summary>
        /// Dictionary where the key is SuperRegion Id
        /// and the value is cached result of static evaluation.
        /// </summary>
        private readonly Dictionary<int, double> superRegionStaticCache
            = new Dictionary<int, double>();

        /// <summary>
        /// Obtains value describing SuperRegion value in
        /// specified game state.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <param name="gameStructure"></param>
        /// <returns></returns>
        public double GetCost(PlayerPerspective currentGameState,
            SuperRegionMin gameStructure)
        {
            double staticCost = GetStaticCost(currentGameState, gameStructure);
            // TODO: better calculation

            return staticCost;
        }

        /// <summary>
        /// Calculates value of given super region
        /// taking only non-changing (static)
        /// fields into account.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <param name="gameStructure"></param>
        /// <returns></returns>
        private double GetStaticCost(PlayerPerspective currentGameState,
            SuperRegionMin gameStructure)
        {
            if (superRegionStaticCache.TryGetValue(gameStructure.Id, out var cachedValue))
            {
                return cachedValue;
            }

            // hint: greater bonus => lesser cost
            double bonusCost = -BonusCoefficient * gameStructure.Bonus;

            var regions = gameStructure
                .RegionsIds
                .Select(x => currentGameState.GetRegion(x))
                .ToList();

            var foreignNeighbourRegionsIds = (from region in regions
                                              from neighbourId in region
                                                  .NeighbourRegionsIds
                                              where region.Id != neighbourId
                                              select neighbourId).ToList();

            // hint: more foreign neighbour it has, worse it is
            int foreignNeighboursCount = foreignNeighbourRegionsIds.Count;
            double foreignNeighboursCost = ForeignNeighboursCoefficient * foreignNeighboursCount;

            // hint: more super region neighbours, the better it is
            int neighbourSuperRegionsCount = foreignNeighbourRegionsIds
                .Select(x => currentGameState.GetRegion(x).SuperRegionId)
                .Distinct().Count();
            double neighbourSuperRegionsCost = neighbourSuperRegionsCount;

            // hint: more regions it has, worse it is (harder to conquer especially at the beginning)
            double regionsCost = SuperRegionsRegionsCountCoeficient * gameStructure.RegionsIds.Length;

           double staticResult = bonusCost + neighbourSuperRegionsCost + regionsCost + foreignNeighboursCount;

            superRegionStaticCache.Add(gameStructure.Id, staticResult);

            return staticResult;
        }
    }
}