namespace GameAi.BotStructures.ActionGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Extensions;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.StructureEvaluators;
    using MCTS;

    /// <summary>
    /// Actions generator for <see cref="MonteCarloTreeSearchBot"/>.
    /// </summary>
    internal class MCTSBotActionsGenerator
        : GameActionsGenerator, IGameActionsGenerator
    {
        public MCTSBotActionsGenerator(
            IRegionMinEvaluator regionMinEvaluator,
            ISuperRegionMinEvaluator superRegionMinEvaluator,
            byte[] playersIds,
            MapMin mapMin) :
            base(new DistanceMatrix(mapMin.RegionsMin),
                regionMinEvaluator, superRegionMinEvaluator,
                playersIds)
        {
        }

        public IReadOnlyList<BotGameTurn> Generate(
            PlayerPerspective currentGameState)
        {
            return GenerateAll(currentGameState);
        }

        private List<BotGameTurn> GenerateAll(
            PlayerPerspective currentGameState)
        {
            byte playerId = currentGameState.PlayerId;
            var gameTurns = new List<BotGameTurn>();

            var deployments = new List<IList<BotDeployment>>();
            deployments.Add(DeployOffensively(currentGameState));
            deployments.Add(DeployToCounterSecurityThreat(currentGameState));
            deployments.Add(DeployToExpand(currentGameState));

            foreach (var botDeployments in deployments
                .Where(x => !x.IsNullOrEmpty()))
            {
                var deploymentCopy = currentGameState.ShallowCopy();
                UpdateGameStateAfterDeploying(ref deploymentCopy, botDeployments);

                var noWaitAggressiveCopy = deploymentCopy.ShallowCopy();
                var noWaitAggressiveAttacks = new List<BotAttack>();
                // generate no wait aggressive attacks
                AttackAggressively(noWaitAggressiveCopy, noWaitAggressiveAttacks);
                AppendRedistributeInlandArmy(noWaitAggressiveCopy, noWaitAggressiveAttacks);
                gameTurns.Add(new BotGameTurn(playerId)
                {
                    Deployments = botDeployments,
                    Attacks = noWaitAggressiveAttacks
                });

                var waitAggressiveCopy = deploymentCopy.ShallowCopy();
                var waitAggressiveAttacks = new List<BotAttack>();
                // generate wait aggressive
                // generate no wait aggressive attacks
                AppendRedistributeInlandArmy(waitAggressiveCopy,
                    waitAggressiveAttacks);
                AttackToExpandSafely(waitAggressiveCopy,
                    waitAggressiveAttacks);
                AttackAggressively(waitAggressiveCopy,
                    waitAggressiveAttacks);
                // is not same as previous attacks
                gameTurns.Add(new BotGameTurn(playerId)
                {
                    Deployments = botDeployments,
                    Attacks = waitAggressiveAttacks
                });

                // play defensive
                var defensiveCopy = deploymentCopy.ShallowCopy();
                var defensiveAttacks = new List<BotAttack>();
                AppendRedistributeInlandArmy(defensiveCopy,
                    defensiveAttacks);
                AttackToExpandSafely(defensiveCopy, defensiveAttacks);
                // is not same as previous attacks
                gameTurns.Add(new BotGameTurn(playerId)
                {
                    Deployments = botDeployments,
                    Attacks = defensiveAttacks
                });
            }

            gameTurns = gameTurns.Distinct().ToList();

            return gameTurns;
        }
    }
}