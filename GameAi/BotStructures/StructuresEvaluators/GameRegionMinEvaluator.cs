namespace GameAi.BotStructures.StructuresEvaluators
{
    using System.Linq;
    using Data.EvaluationStructures;
    using Interfaces.Evaluators.StructureEvaluators;

    public class GameRegionMinEvaluator : IRegionMinEvaluator
    {
        private const double NeighboursCoefficient = 4;
        private const double SuperRegionCoefficient = 3;
        private const double ArmyCoefficient = 4;
        private const double BonusCoefficient = 10;

        private readonly ISuperRegionMinEvaluator superRegionMinEvaluator;

        public GameRegionMinEvaluator(ISuperRegionMinEvaluator superRegionMinEvaluator)
        {
            this.superRegionMinEvaluator = superRegionMinEvaluator;
        }
        
        public double GetValue(PlayerPerspective currentGameState, RegionMin gameStructure)
        {
            double staticValue = GetStaticValue(gameStructure);
            ref SuperRegionMin superRegion = ref currentGameState.GetSuperRegion(gameStructure.SuperRegionId);
            var myRegions = currentGameState.GetMyRegions().ToList();

            // hint: more regions of the super region the region owner has,
            // the higher value it has
            int superRegionsRegionsOwningCount = superRegion.RegionsIds
                .Select(x => currentGameState.GetRegion(x))
                .Count(x => x.OwnerId == currentGameState.PlayerId);
            double superRegionValue;
            if (superRegion.OwnerId == gameStructure.OwnerId)
            {
                superRegionValue = SuperRegionCoefficient * superRegionsRegionsOwningCount
                                   + BonusCoefficient * superRegion.Bonus
                                   + superRegionMinEvaluator.GetValue(currentGameState, superRegion);
            }
            else
            {
                // hint: super region value has influence on how good the given super region is
                superRegionValue = SuperRegionCoefficient * superRegionsRegionsOwningCount
                                   + superRegionMinEvaluator.GetValue(currentGameState, superRegion);
            }

            // hint: larger army => higher (if mine) or lower (if not mine) the value

            

            double dynamicValue = superRegionValue;

            return dynamicValue + staticValue;
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