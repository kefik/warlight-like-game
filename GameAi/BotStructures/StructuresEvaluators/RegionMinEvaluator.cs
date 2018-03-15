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
    internal class RegionMinEvaluator : IRegionMinEvaluator
    {
        private const double NeighboursCoefficient = 1;
        private const double ArmyCoefficient = 3;
        private const double SuperRegionCoefficient = 2;

        /// <summary>
        /// Returns value of given <see cref="RegionMin"/> in
        /// the context of the <see cref="PlayerPerspective"/>.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <param name="gameStructure"></param>
        /// <returns></returns>
        public double GetValue(PlayerPerspective currentGameState, RegionMin gameStructure)
        {
            double staticResult = CalculateStaticResult(gameStructure);

            // calculate dynamic part

            // calculate army
            double army = ArmyCoefficient * gameStructure.Army;

            // calculate super region influence => more regions from super region I have, the greater the SuperRegion value is
            ref SuperRegionMin superRegion = ref currentGameState.GetSuperRegion(gameStructure.SuperRegionId);
            int myRegionsInSuperRegionCount = (from regionId in superRegion
                                                    .RegionsIds.Select(x => currentGameState.GetRegion(x))
                                               where regionId.OwnerId == currentGameState.PlayerId
                                               select regionId).Count();

            double superRegionValue = SuperRegionCoefficient * myRegionsInSuperRegionCount;
            if (myRegionsInSuperRegionCount == superRegion.RegionsIds.Length)
            {
                superRegionValue += SuperRegionCoefficient * superRegion.Bonus;
            }

            double result = staticResult + army + superRegionValue;

            return result;
        }

        private double CalculateStaticResult(RegionMin gameStructure)
        {
            int neighboursCount = gameStructure.NeighbourRegionsIds.Length;

            double staticResult = -NeighboursCoefficient * neighboursCount;

            return staticResult;
        }
    }
}