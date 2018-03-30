#if DEBUG
#define LOG_EVALUATOR
#endif
namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ActionGenerators;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Data.Restrictions;
    using Interfaces.Evaluators;
    using Interfaces.Evaluators.StructureEvaluators;
    using StructuresEvaluators;

    /// <summary>
    ///     Component handling Monte Carlo tree search evaluation.
    ///     It's purpose is to handle parallel evaluation,
    ///     action generators creation based on information
    ///     coming from bot instance.
    /// </summary>
    internal class MCTSEvaluationHandler : IDisposable
    {
        private CancellationTokenSource CancellationTokenSource
        {
            get;
            set;
        }

        private MCTSTreeHandler[] treeHandlers;
        private readonly byte enemyPlayerId;

        public MCTSEvaluationHandler(
            PlayerPerspective playerPerspective, byte enemyPlayerId,
            Restrictions restrictions)
        {
            this.enemyPlayerId = enemyPlayerId;
            Initialize(playerPerspective, restrictions);
        }

        /// <summary>
        ///     Initializes components for MCTS evaluation.
        /// </summary>
        /// <param name="initialGameState">Initial game state of the evaluation.</param>
        /// <param name="restrictions">Restrictions for the game.</param>
        /// <remarks>
        ///     Must be called only if the evaluation is stopped,
        ///     otherwise it can cause unpredicted behaviour.
        /// </remarks>
        public void Initialize(PlayerPerspective initialGameState,
            Restrictions restrictions)
        {
            DistanceMatrix distanceMatrix =
                new DistanceMatrix(initialGameState.MapMin
                    .RegionsMin);
            ISuperRegionMinEvaluator
                gameBeginningSuperRegionMinEvaluator =
                    new GameBeginningSuperRegionMinEvaluator(
                        initialGameState.MapMin,
                        superRegionsRegionsCountCoefficient: 3,
                        foreignNeighboursCoefficient: 5,
                        bonusCoefficient: 3);
            IRegionMinEvaluator gameBeginningRegionMinEvaluator =
                new GameBeginningRegionMinEvaluator(
                    gameBeginningSuperRegionMinEvaluator,
                    distanceMatrix, clusterCoefficient: 50,
                    superRegionCoefficient: 3);

            ISuperRegionMinEvaluator gameSuperRegionMinEvaluator =
                new GameSuperRegionMinEvaluator(
                    initialGameState.MapMin, bonusCoefficient: 3,
                    conqueredCoefficient: 30,
                    foreignNeighboursCoefficient: 3,
                    superRegionsRegionsCountCoefficient: 5);
            IRegionMinEvaluator gameRegionMinEvaluator =
                new GameRegionMinEvaluator(
                    gameSuperRegionMinEvaluator,
                    superRegionCoefficient: 20);

            IRoundEvaluator roundEvaluator = new RoundEvaluator();
            IPlayerPerspectiveEvaluator gamePlayerPerspectiveEvaluator
                = new PlayerPerspectiveEvaluator(
                    gameRegionMinEvaluator, armyCoefficient: 10);

            SelectRegionActionsGenerator selectRegionActionsGenerator
                = new SelectRegionActionsGenerator(
                    gameBeginningRegionMinEvaluator,
                    restrictions.GameBeginningRestrictions);
            MCTSBotActionsGenerator gameActionsGenerator =
                new MCTSBotActionsGenerator(gameRegionMinEvaluator,
                    initialGameState.MapMin);

#if LOG_EVALUATOR
            Debug.WriteLine($"INITIAL POSITION (BOT {initialGameState.PlayerId})");
            if (initialGameState.MapMin.IsGameBeginning())
            {
                Debug.WriteLine("Game beginning phase");
                Debug.WriteLine("SuperRegions:");
                foreach (SuperRegionMin superRegion in
                    initialGameState.MapMin.SuperRegionsMin.OrderBy(
                        x => x.Name))
                {
                    Debug.WriteLine(
                        $"Name: {superRegion.Name}, Value: " +
                        $"{gameBeginningSuperRegionMinEvaluator.GetValue(initialGameState, superRegion):F1}");
                }
                Debug.WriteLine("");

                Debug.WriteLine($"Regions:");
                foreach (RegionMin region in initialGameState.MapMin
                    .RegionsMin.OrderBy(x => x.Name))
                {
                    Debug.WriteLine($"Name: {region.Name}, Value: " +
                                    $"{gameBeginningRegionMinEvaluator.GetValue(initialGameState, region):F1}");
                }
                Debug.WriteLine("");
            }
            else
            {
                Debug.WriteLine("SuperRegions:");
                foreach (SuperRegionMin superRegion in
                    initialGameState.MapMin.SuperRegionsMin.OrderBy(
                        x => x.Name))
                {
                    Debug.WriteLine(
                        $"Name: {superRegion.Name}, Value: " +
                        $"{gameSuperRegionMinEvaluator.GetValue(initialGameState, superRegion):F1}");
                }
                Debug.WriteLine("");

                Debug.WriteLine($"Regions:");
                foreach (RegionMin region in initialGameState.MapMin
                    .RegionsMin.OrderBy(x => x.Name))
                {
                    Debug.WriteLine($"Name: {region.Name}, Value: " +
                                    $"{gameRegionMinEvaluator.GetValue(initialGameState, region):F1}");
                }
                Debug.WriteLine("");
            }
#endif

            // clear what was left after previous evaluation
            CancellationTokenSource = new CancellationTokenSource();
            ClearEvaluationCache();

            // # tree handlers == how much parallely do we want it to evaluate
            //treeHandlers = new MCTSTreeHandler[1];
            treeHandlers = new MCTSTreeHandler[Environment.ProcessorCount];

            for (int index = 0; index < treeHandlers.Length; index++)
            {
                treeHandlers[index] =
                    new MCTSTreeHandler(initialGameState,
                        enemyPlayerId, gameActionsGenerator,
                        selectRegionActionsGenerator,
                        gamePlayerPerspectiveEvaluator,
                        roundEvaluator);
            }
        }


        public BotTurn GetBestMove()
        {
            // merge the trees and get the most visited node
            var nodesToCompare = treeHandlers.Select(x => x.Tree.Root)
                .SelectMany(x => x.Children);

            // groups by turns with summed counts
            var turnsWithSums =
            (from node in nodesToCompare
             group node by node.Action).Select(x => new
            {
                Turn = x.Key,
                WinCount = x.Sum(y => y.WinCount),
                VisitCount = x.Sum(y => y.VisitCount)
            });

            // best move by visitcount and wincount
            var turnsByVisitCount = turnsWithSums
                .OrderByDescending(x => x.VisitCount)
                .ThenByDescending(x => x.WinCount);

#if LOG_EVALUATOR
            Debug.WriteLine("EVALUATION RESULT");
            foreach (var mctsTreeNode in turnsByVisitCount)
            {
                var gameState = nodesToCompare.First().GameState;
                Debug.WriteLine($"{mctsTreeNode.WinCount:F}/{mctsTreeNode.VisitCount}");
                Debug.WriteLine($"ACTIONS:");

                switch (mctsTreeNode.Turn)
                {
                    case BotGameBeginningTurn gameBeginningTurn:
                        Debug.WriteLine($"Seizes:");
                        foreach (int seizedRegionsId in gameBeginningTurn.SeizedRegionsIds)
                        {
                            Debug.WriteLine($"{gameState.GetRegion(seizedRegionsId).Name}");
                        }
                        break;
                    case BotGameTurn gameTurn:
                        var deployments = gameTurn.Deployments;
                        Debug.WriteLine("Deployments:");
                        foreach (BotDeployment botDeployment in deployments)
                        {
                            var region = gameState.GetRegion(botDeployment.RegionId);
                            Debug.WriteLine($"Region: {region.Name}, New army: {botDeployment.Army}");
                        }

                        var attacks = gameTurn.Attacks;
                        Debug.WriteLine("Attacks:");
                        foreach (BotAttack botAttack in attacks)
                        {
                            var attackingRegion = gameState.GetRegion(botAttack.AttackingRegionId);
                            var defendingRegion = gameState.GetRegion(botAttack.DefendingRegionId);
                            Debug.WriteLine(
                                $"{attackingRegion.Name} -> {defendingRegion.Name}, Army: {botAttack.AttackingArmy}");
                        }
                        break;
                }
                Debug.WriteLine("");
            }
            Debug.WriteLine("");
            Debug.WriteLine("");
#endif

            return turnsByVisitCount.Select(x => x.Turn).First();
        }

        /// <summary>
        ///     Asynchronously starts MCTS evaluation in parallel.
        /// </summary>
        /// <returns></returns>
        public async Task StartEvaluationAsync()
        {
            var tasks = new Task[treeHandlers.Length];

            CancellationTokenSource = new CancellationTokenSource();

            for (int i = 0; i < tasks.Length; i++)
            {
                MCTSTreeHandler treeHandler = treeHandlers[i];

                tasks[i] =
                    Task.Factory.StartNew(
                        () => treeHandler.StartEvaluation(
                            CancellationTokenSource.Token),
                        CancellationToken.None,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Default);
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        ///     Stops the evaluation.
        /// </summary>
        public void Stop()
        {
            CancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Asynchronously clears cache after evaluation.
        /// </summary>
        /// <returns></returns>
        public async Task ClearEvaluationCacheAsync()
        {
            var tasks = new Task[treeHandlers.Length];

            for (int i = 0; i < tasks.Length; i++)
            {
                int i1 = i;
                tasks[i] = Task.Run(() => treeHandlers[i1].Tree
                    .FreeEntireTree());
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        ///     Clears evaluation cache.
        /// </summary>
        public void ClearEvaluationCache()
        {
            if (treeHandlers != null)
            {
                foreach (MCTSTreeHandler treeHandler in treeHandlers)
                {
                    treeHandler.Tree.FreeEntireTree();
                }
            }
        }

        /// <summary>
        ///     Disposes resources, clearing evaluation cache.
        /// </summary>
        public void Dispose()
        {
            ClearEvaluationCache();
        }

        ~MCTSEvaluationHandler()
        {
            ClearEvaluationCache();
        }
    }
}