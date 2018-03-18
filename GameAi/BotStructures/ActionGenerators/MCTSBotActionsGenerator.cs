namespace GameAi.BotStructures.ActionGenerators
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.StructureEvaluators;

    public class MCTSBotActionsGenerator : IGameActionsGenerator
    {
        private readonly IRegionMinEvaluator regionMinEvaluator;
        private readonly byte enemyPlayerId;

        public MCTSBotActionsGenerator(IRegionMinEvaluator regionMinEvaluator,
            byte enemyPlayerId)
        {
            this.regionMinEvaluator = regionMinEvaluator;
            this.enemyPlayerId = enemyPlayerId;
        }

        public IReadOnlyList<BotGameTurn> Generate(PlayerPerspective currentGameState)
        {
            // deploy

            // hint: threatened regions == enemy is near them
            var threatenedRegions = from myRegion in currentGameState.GetMyRegions()
                                    where currentGameState.GetNeighbourRegions(myRegion.Id)
                                        .Any(x => x.OwnerId == enemyPlayerId)
                                    select myRegion;

            // hint: regions that are near ones I'd like to attack
            var regionsNearRegionstoAttack = from myRegion in currentGameState.GetMyRegions()
                                             from neighbour in currentGameState.GetNeighbourRegions(myRegion.Id)
                                             orderby regionMinEvaluator.GetValue(currentGameState, neighbour) descending ,
                                                regionMinEvaluator.GetValue(currentGameState, myRegion) descending 
                                             select myRegion;

            // attack

            throw new System.NotImplementedException();
        }
    }
}