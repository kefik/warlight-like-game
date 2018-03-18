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
        private const double ClusterCoefficient = 2;

        private readonly DistanceMatrix distanceMatrix;

        private readonly ISuperRegionMinEvaluator superRegionMinEvaluator;

        public GameBeginningRegionMinEvaluator(ISuperRegionMinEvaluator superRegionMinEvaluator,
            DistanceMatrix distanceMatrix)
        {
            this.superRegionMinEvaluator = superRegionMinEvaluator;
            this.distanceMatrix = distanceMatrix;
        }

        /// <summary>
        /// Returns value of given <see cref="RegionMin"/> in
        /// the context of the <see cref="PlayerPerspective"/>.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <param name="gameStructure"></param>
        /// <returns></returns>
        public double GetValue(PlayerPerspective currentGameState, RegionMin gameStructure)
        {
            double staticValue = GetStaticValue(gameStructure);
            ref SuperRegionMin superRegion = ref currentGameState.GetSuperRegion(gameStructure.SuperRegionId);
            var myRegions = currentGameState.GetMyRegions().ToList();

            // hint: super region value has influence on how good the given super region is
            double superRegionValue = SuperRegionCoefficient
                * superRegionMinEvaluator.GetValue(currentGameState, superRegion);

            // hint: clustered regions are not good to choose
            // greater from owned neighbour, lesser the cost
            double clusterValue = 0;
            if (currentGameState.IsNeighbourToAnyMyRegion(gameStructure))
            {
                clusterValue += ClusterCoefficient
                    // get distance closest region of the same player
                    * currentGameState.MapMin.RegionsMin
                        .Where(x => x.OwnerId == currentGameState.PlayerId)
                        .Where(x => x.Id != gameStructure.Id)
                        .Min(x => distanceMatrix.GetDistance(x.Id, gameStructure.Id));
            }

            double dynamicCost = superRegionValue - clusterValue;

            return dynamicCost + staticValue;
        }

        /// <summary>
        /// Gets static cost of <see cref="RegionMin"/> structure.
        /// </summary>
        /// <param name="gameStructure"></param>
        /// <returns></returns>
        private double GetStaticValue(RegionMin gameStructure)
        {
            int neighboursCount = gameStructure.NeighbourRegionsIds.Length;

            // hint: more neighbours => harder it is to defend the region
            double staticResult = -NeighboursCoefficient * neighboursCount;

            return staticResult;
        }
    }
}