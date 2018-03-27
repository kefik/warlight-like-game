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
        private readonly double bonusCoefficient;

        private readonly ISuperRegionMinEvaluator superRegionMinEvaluator;

        public GameRegionMinEvaluator(ISuperRegionMinEvaluator superRegionMinEvaluator,
            double bonusCoefficient, double superRegionCoefficient)
        {
            this.superRegionMinEvaluator = superRegionMinEvaluator;

            this.superRegionCoefficient = superRegionCoefficient;
            this.bonusCoefficient = bonusCoefficient;
        }
        
        public double GetValue(PlayerPerspective currentGameState, RegionMin gameStructure)
        {
            ref SuperRegionMin superRegion = ref currentGameState
                .GetSuperRegion(gameStructure.SuperRegionId);

            byte regionOwner = gameStructure.OwnerId;
            // hint: more regions of the super region the region owner has,
            // the higher value it has
            int superRegionsRegionsOwningCount = superRegion.RegionsIds
                .Select(x => currentGameState.GetRegion(x))
                .Count(x => x.OwnerId == regionOwner);
            double superRegionValue;
            
            if (superRegion.OwnerId == gameStructure.OwnerId && gameStructure.OwnerId != 0)
            {
                // hint: region of completed super region has higher value
                superRegionValue = superRegionCoefficient
                                    * superRegionsRegionsOwningCount
                                   + bonusCoefficient * superRegion.Bonus
                                   + superRegionMinEvaluator
                                    .GetValue(currentGameState, superRegion);
            }
            else
            {
                // hint: super region value has influence on how good the given super region is
                superRegionValue = superRegionCoefficient 
                                    * superRegionsRegionsOwningCount
                                   + superRegionMinEvaluator
                                    .GetValue(currentGameState, superRegion);
            }

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