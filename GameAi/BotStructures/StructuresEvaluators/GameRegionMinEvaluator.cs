﻿namespace GameAi.BotStructures.StructuresEvaluators
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Data;
    using Data.EvaluationStructures;
    using Interfaces.Evaluators.StructureEvaluators;

    public class GameRegionMinEvaluator : IRegionMinEvaluator
    {
        private const double NeighboursCoefficient = 4;
        private const double SuperRegionCoefficient = 3;
        private const double ArmyCoefficient = 4;
        private const double BonusCoefficient = 15;

        private readonly ISuperRegionMinEvaluator superRegionMinEvaluator;

        public GameRegionMinEvaluator(ISuperRegionMinEvaluator superRegionMinEvaluator)
        {
            this.superRegionMinEvaluator = superRegionMinEvaluator;
        }
        
        public double GetValue(PlayerPerspective currentGameState, RegionMin gameStructure)
        {
            double staticValue = GetStaticValue(gameStructure);
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