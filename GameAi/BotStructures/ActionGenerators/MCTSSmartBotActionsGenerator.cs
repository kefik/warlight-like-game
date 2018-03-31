namespace GameAi.BotStructures.ActionGenerators
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.StructureEvaluators;

    internal class MCTSSmartBotActionsGenerator
        : GameActionsGenerator, IGameActionsGenerator
    {
        public MCTSSmartBotActionsGenerator(
            DistanceMatrix distanceMatrix,
            IRegionMinEvaluator regionMinEvaluator) : base(
            distanceMatrix, regionMinEvaluator)
        {
        }

        public IReadOnlyList<BotGameTurn> Generate(
            PlayerPerspective currentGameState)
        {
            IEnumerable<BotDeployment> deployments =
                DeployOffensively(currentGameState);
            deployments =
                deployments.Union(
                    DeployToCounterSecurityThreat(currentGameState));
            deployments =
                deployments.Union(DeployToExpand(currentGameState));

            foreach (var deployment in deployments)
            {
                var deploymentCopy = currentGameState.ShallowCopy();
                UpdateGameStateAfterDeploying(ref deploymentCopy,
                    new List<BotDeployment>() {deployment});
            }
            throw new System.NotImplementedException();
        }

        private IList<BotAttack> SmartAggressive(
            PlayerPerspective currentGameState)
        {
            var attacks = new List<BotAttack>();

            // enemies ordered by army
            var enemyRegions = currentGameState.MapMin.RegionsMin
                .Where(x => x.OwnerId != 0 &&
                            x.OwnerId != currentGameState.PlayerId)
                .OrderByDescending(x => x.Army);

            foreach (var enemyRegion in enemyRegions)
            {
                var myNeighbours = enemyRegion.NeighbourRegionsIds
                    .Select(x => currentGameState.GetRegion(x))
                    .Where(
                        x => x.OwnerId == currentGameState.PlayerId)
                    .ToList();

            }
            

            return attacks;
        }
    }
}