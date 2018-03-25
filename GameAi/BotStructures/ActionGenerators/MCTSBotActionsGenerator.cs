namespace GameAi.BotStructures.ActionGenerators
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.StructureEvaluators;
    using MCTS;

    /// <summary>
    /// Actions generator for <see cref="MonteCarloTreeSearchBot"/>.
    /// </summary>
    public class MCTSBotActionsGenerator : GameActionsGenerator, IGameActionsGenerator
    {
        public MCTSBotActionsGenerator(IRegionMinEvaluator regionMinEvaluator,
            MapMin mapMin) : base(new DistanceMatrix(mapMin.RegionsMin), regionMinEvaluator)
        {
        }

        public IReadOnlyList<BotGameTurn> Generate(PlayerPerspective currentGameState)
        {
            return GenerateAll(currentGameState);
        }

        private List<BotGameTurn> GenerateAll(PlayerPerspective currentGameState)
        {
            byte playerId = currentGameState.PlayerId;
            var gameTurns = new List<BotGameTurn>();

            var deployments = DeployToAll(currentGameState);

            foreach (BotDeployment botDeployment in deployments)
            {
                var deploymentCopy = currentGameState.ShallowCopy();

                var deployment = new List<BotDeployment>() {botDeployment};
                UpdateGameStateAfterDeploying(ref deploymentCopy, deployment);

                var noWaitAggressiveCopy = deploymentCopy.ShallowCopy();
                var attacks = new List<BotAttack>();
                // generate no wait aggressive attacks
                AttackAggressively(noWaitAggressiveCopy, attacks);
                AppendRedistributeInlandArmy(noWaitAggressiveCopy, attacks);
                gameTurns.Add(new BotGameTurn(playerId)
                {
                    Deployments = deployment,
                    Attacks = attacks
                });

                var waitAggressiveCopy = deploymentCopy.ShallowCopy();
                attacks = new List<BotAttack>();
                // generate wait aggressive
                // generate no wait aggressive attacks
                AttackSafely(waitAggressiveCopy, attacks);
                AppendRedistributeInlandArmy(waitAggressiveCopy, attacks);
                AttackAggressively(waitAggressiveCopy, attacks);
                gameTurns.Add(new BotGameTurn(playerId)
                {
                    Deployments = deployment,
                    Attacks = attacks
                });

                // play defensive
                var defensiveCopy = deploymentCopy.ShallowCopy();
                attacks = new List<BotAttack>();
                AttackSafely(defensiveCopy, attacks);
                AppendRedistributeInlandArmy(defensiveCopy, attacks);
                gameTurns.Add(new BotGameTurn(playerId)
                {
                    Deployments = deployment,
                    Attacks = attacks
                });
            }

            return gameTurns;
        }
    }
}