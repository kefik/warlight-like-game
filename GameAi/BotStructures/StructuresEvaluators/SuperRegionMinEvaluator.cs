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
    internal class SuperRegionMinEvaluator : ISuperRegionMinEvaluator
    {
        private const double SuperRegionsRegionsCountCoeficient = 5;
        private const double ForeignNeighboursCoefficient = 4;

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
        public double GetValue(PlayerPerspective currentGameState,
            SuperRegionMin gameStructure)
        {
            double staticResult;
            if (superRegionStaticCache.TryGetValue(gameStructure.Id, out var cachedValue))
            {
                staticResult = cachedValue;
            }
            else
            {
                staticResult = CalculateStaticResult(currentGameState,
                    gameStructure);
                superRegionStaticCache.Add(gameStructure.Id, staticResult);
            }

            // TODO: better calculation

            return staticResult;
        }

        /// <summary>
        /// Calculates value of given super region
        /// taking only non-changing (static)
        /// fields into account.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <param name="gameStructure"></param>
        /// <returns></returns>
        private double CalculateStaticResult(PlayerPerspective currentGameState,
            SuperRegionMin gameStructure)
        {
            var regions = gameStructure
                .RegionsIds
                .Select(x => currentGameState.GetRegion(x))
                .ToList();

            var foreignNeighbourRegionsIds = (from region in regions
                                              from neighbourId in region
                                                  .NeighbourRegionsIds
                                              where region.Id != neighbourId
                                              select neighbourId).ToList();

            int foreignNeighboursCount = foreignNeighbourRegionsIds.Count;

            int neighbourSuperRegionsCount = foreignNeighbourRegionsIds
                .Select(x => currentGameState.GetRegion(x).SuperRegionId)
                .Distinct().Count();

            double staticResult =
                (double)gameStructure.Bonus
                + neighbourSuperRegionsCount
                - SuperRegionsRegionsCountCoeficient * gameStructure.RegionsIds.Length
                - ForeignNeighboursCoefficient * foreignNeighboursCount;

            return staticResult;
        }
    }
}