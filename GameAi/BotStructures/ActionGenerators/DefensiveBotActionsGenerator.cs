namespace GameAi.BotStructures.ActionGenerators
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.StructureEvaluators;

    public class DefensiveBotActionsGenerator : GameActionsGenerator, IGameActionsGenerator
    {
        private readonly IRegionMinEvaluator regionMinEvaluator;

        public DefensiveBotActionsGenerator(IRegionMinEvaluator regionMinEvaluator)
        {
            this.regionMinEvaluator = regionMinEvaluator;
        }

        public IReadOnlyList<BotGameTurn> Generate(PlayerPerspective currentGameState)
        {
            var botGameTurns = new List<BotGameTurn>();

            // hint: protect regions that might get attacked
            var regionsToProtect = from region in currentGameState.GetMyRegions()
                                   from neighbour in region.NeighbourRegionsIds
                                       .Select(x => currentGameState.GetRegion(x))
                                   where neighbour.OwnerId != currentGameState.PlayerId
                                   where neighbour.OwnerId != 0
                                   // sort by region to protect value, then by neighbour army
                                   orderby
                                    regionMinEvaluator.GetValue(currentGameState, region) descending,
                                    neighbour.Army descending
                                   select region;

            foreach (var regionMin in regionsToProtect)
            {
                var botGameTurn = new BotGameTurn(currentGameState.PlayerId);

                var neighboursArmiesSum = regionMin.NeighbourRegionsIds
                    .Select(x => currentGameState.GetRegion(x))
                    .Where(x => x.OwnerId != 0 && x.OwnerId != currentGameState.PlayerId)
                    .Sum(x => x.Army);
                
            }


            throw new System.NotImplementedException();
        }
    }
}