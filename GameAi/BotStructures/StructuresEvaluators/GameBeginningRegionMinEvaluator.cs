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
        private readonly double superRegionCoefficient;
        private readonly double clusterCoefficient;

        private readonly DistanceMatrix distanceMatrix;

        private readonly ISuperRegionMinEvaluator superRegionMinEvaluator;

        public GameBeginningRegionMinEvaluator(ISuperRegionMinEvaluator superRegionMinEvaluator,
            DistanceMatrix distanceMatrix,
            double clusterCoefficient,
            double superRegionCoefficient)
        {
            this.clusterCoefficient = clusterCoefficient;
            this.superRegionCoefficient = superRegionCoefficient;
            this.superRegionMinEvaluator = superRegionMinEvaluator;
            this.distanceMatrix = distanceMatrix;
        }

        public GameBeginningRegionMinEvaluator(ISuperRegionMinEvaluator superRegionMinEvaluator,
            MapMin map, double clusterCoefficient,
            double superRegionCoefficient) : this(superRegionMinEvaluator, new DistanceMatrix(map.RegionsMin), clusterCoefficient,
                superRegionCoefficient)
        {
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
            ref SuperRegionMin superRegion = ref currentGameState.GetSuperRegion(gameStructure.SuperRegionId);

            // hint: super region value has influence on how good the given super region is
            double superRegionValue = superRegionCoefficient
                * superRegionMinEvaluator.GetValue(currentGameState, superRegion);

            // hint: clustered regions are not good to choose
            // greater from owned neighbour, lesser the cost
            double clusterValue = 0;
            var myOldRegions = currentGameState.MapMin.RegionsMin
                .Where(x => x.OwnerId == currentGameState.PlayerId)
                .Where(x => x.Id != gameStructure.Id).ToList();

            if (myOldRegions.Count != 0)
            {
                clusterValue += clusterCoefficient
                                // get distance closest region of the same player
                                * Math.Min(
                                    myOldRegions
                                    .Min(x => distanceMatrix.GetDistance(x.Id, gameStructure.Id)),
                                    distanceMatrix.GetMaximumDistance() / 2);
            }

            double dynamicCost = superRegionValue + clusterValue;

            return dynamicCost;
        }
    }
}