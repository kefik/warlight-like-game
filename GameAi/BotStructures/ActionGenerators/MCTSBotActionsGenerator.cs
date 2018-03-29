namespace GameAi.BotStructures.ActionGenerators
{
    using System;
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
    internal class MCTSBotActionsGenerator : GameActionsGenerator, IGameActionsGenerator
    {
        public MCTSBotActionsGenerator(IRegionMinEvaluator regionMinEvaluator,
            MapMin mapMin) : base(new DistanceMatrix(mapMin.RegionsMin), regionMinEvaluator)
        {
        }

        public IReadOnlyList<BotGameTurn> Generate(PlayerPerspective currentGameState)
        {
            // TODO: enemy region nearby => sort it out

            return GenerateAll(currentGameState);
        }

        private List<BotGameTurn> GenerateAll(PlayerPerspective currentGameState)
        {
            byte playerId = currentGameState.PlayerId;
            var gameTurns = new List<BotGameTurn>();

            IEnumerable<BotDeployment> deployments = DeployOffensively(currentGameState);
            deployments = deployments.Union(DeployToCounterSecurityThreat(currentGameState));

            foreach (BotDeployment botDeployment in deployments)
            {
                var deploymentCopy = currentGameState.ShallowCopy();

                var deployment = new List<BotDeployment> { botDeployment };
                UpdateGameStateAfterDeploying(ref deploymentCopy, deployment);

                var noWaitAggressiveCopy = deploymentCopy.ShallowCopy();
                var noWaitAggressiveAttacks = new List<BotAttack>();
                // generate no wait aggressive attacks
                AttackAggressively(noWaitAggressiveCopy, noWaitAggressiveAttacks);
                AppendRedistributeInlandArmy(noWaitAggressiveCopy, noWaitAggressiveAttacks);
                gameTurns.Add(new BotGameTurn(playerId)
                {
                    Deployments = deployment,
                    Attacks = noWaitAggressiveAttacks
                });

                var waitAggressiveCopy = deploymentCopy.ShallowCopy();
                var waitAggressiveAttacks = new List<BotAttack>();
                // generate wait aggressive
                // generate no wait aggressive attacks
                AppendRedistributeInlandArmy(waitAggressiveCopy, waitAggressiveAttacks);
                AttackNeutralSafely(waitAggressiveCopy, waitAggressiveAttacks);
                AttackAggressively(waitAggressiveCopy, waitAggressiveAttacks);
                gameTurns.Add(new BotGameTurn(playerId)
                {
                    Deployments = deployment,
                    Attacks = waitAggressiveAttacks
                });

                // play defensive
                var defensiveCopy = deploymentCopy.ShallowCopy();
                var defensiveAttacks = new List<BotAttack>();
                AppendRedistributeInlandArmy(defensiveCopy, defensiveAttacks);
                AttackNeutralSafely(defensiveCopy, defensiveAttacks);
                gameTurns.Add(new BotGameTurn(playerId)
                {
                    Deployments = deployment,
                    Attacks = defensiveAttacks
                });
            }

            return gameTurns;
        }
    }
}