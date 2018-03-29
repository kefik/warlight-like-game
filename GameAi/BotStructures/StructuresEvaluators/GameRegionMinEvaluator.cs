namespace GameAi.BotStructures.StructuresEvaluators
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Data;
    using Data.EvaluationStructures;
    using Interfaces.Evaluators.StructureEvaluators;

    public class GameRegionMinEvaluator : IRegionMinEvaluator
    {
        private readonly double superRegionCoefficient;

        private readonly ISuperRegionMinEvaluator superRegionMinEvaluator;

        public GameRegionMinEvaluator(ISuperRegionMinEvaluator superRegionMinEvaluator, double superRegionCoefficient)
        {
            this.superRegionMinEvaluator = superRegionMinEvaluator;

            this.superRegionCoefficient = superRegionCoefficient;
        }
        
        public double GetValue(PlayerPerspective currentGameState, RegionMin gameStructure)
        {
            ref SuperRegionMin superRegion = ref currentGameState
                .GetSuperRegion(gameStructure.SuperRegionId);

            // hint: more regions of the super region the region owner has,
            // the higher value it has
            int superRegionsRegionsOwningCount = superRegion.RegionsIds
                .Select(x => currentGameState.GetRegion(x))
                .Count(x => x.OwnerId == currentGameState.PlayerId);
            // hint: region of completed super region has higher value
            double superRegionValue = superRegionCoefficient
                               * superRegionsRegionsOwningCount
                               + superRegionMinEvaluator
                                   .GetValue(currentGameState, superRegion);
            
            // enemy has more regions of this super region
            if (gameStructure.OwnerId != 0
                && gameStructure.OwnerId != currentGameState.PlayerId)
            {
                int enemyOwnedCount = superRegion.RegionsIds
                    .Select(x => currentGameState.GetRegion(x))
                    .Count(x => x.OwnerId == gameStructure.OwnerId);
                // hint: super region value has influence on how good the given super region is
                superRegionValue += superRegionCoefficient
                                   * enemyOwnedCount
                                   + superRegionMinEvaluator
                                       .GetValue(currentGameState, superRegion);
            }

            // TODO: regions with better neighbours have higher value

            // hint: mine or enemy region has higher value than not-occupied
            switch (gameStructure.GetOwnerPerspective(currentGameState.PlayerId))
            {
                case OwnerPerspective.Unoccupied:
                    break;
                case OwnerPerspective.Enemy:
                case OwnerPerspective.Mine:
                    superRegionValue *= 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            

            double dynamicValue = superRegionValue;

            return dynamicValue;
        }
    }
}