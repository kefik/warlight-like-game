namespace GameAi.BotStructures.StructuresEvaluators
{
    using System;
    using System.Linq;
    using Data;
    using Data.EvaluationStructures;
    using Interfaces.Evaluators.StructureEvaluators;

    /// <summary>
    /// Component that can evaluate <see cref="RegionMin"/>.
    /// </summary>
    internal class GameBeginningRegionMinEvaluator : IRegionMinEvaluator
    {
        private const double NeighboursCoefficient = 1;
        private const double SuperRegionCoefficient = 3;
        private const double ClusterCoefficient = 3;

        private readonly ISuperRegionMinEvaluator superRegionMinEvaluator;

        public GameBeginningRegionMinEvaluator(ISuperRegionMinEvaluator superRegionMinEvaluator)
        {
            this.superRegionMinEvaluator = superRegionMinEvaluator;
        }

        /// <summary>
        /// Returns value of given <see cref="RegionMin"/> in
        /// the context of the <see cref="PlayerPerspective"/>.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <param name="gameStructure"></param>
        /// <returns></returns>
        public double GetCost(PlayerPerspective currentGameState, RegionMin gameStructure)
        {
            double staticCost = GetStaticCost(gameStructure);
            ref SuperRegionMin superRegion = ref currentGameState.GetSuperRegion(gameStructure.SuperRegionId);
            var myRegions = currentGameState.GetMyRegions().ToList();

            // hint: super region value has influence on how good the given super region is
            double superRegionValue = SuperRegionCoefficient
                * superRegionMinEvaluator.GetCost(currentGameState, superRegion);

            // hint: clustered regions are not good to choose
            // greater from owned neighbour, lesser the cost
            // TODO: distance <= 3 means clustered
            double clusterValue = 0;
            if (currentGameState.IsNeighbourToAnyMyRegion(gameStructure))
            {
                clusterValue -= ClusterCoefficient; //TODO: * distance
            }

            double dynamicCost = superRegionValue;

            return dynamicCost + staticCost;
        }

        /// <summary>
        /// Gets static cost of <see cref="RegionMin"/> structure.
        /// </summary>
        /// <param name="gameStructure"></param>
        /// <returns></returns>
        private double GetStaticCost(RegionMin gameStructure)
        {
            int neighboursCount = gameStructure.NeighbourRegionsIds.Length;

            // hint: more neighbours => harder it is to defend the region
            double staticResult = NeighboursCoefficient * neighboursCount;

            return staticResult;
        }
    }
}