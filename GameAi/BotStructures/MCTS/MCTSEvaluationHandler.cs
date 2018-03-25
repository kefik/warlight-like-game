namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ActionGenerators;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Data.Restrictions;
    using Interfaces;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators;
    using Interfaces.Evaluators.NodeEvaluators;
    using Interfaces.Evaluators.StructureEvaluators;
    using StructuresEvaluators;

    /// <summary>
    /// Component handling Monte Carlo tree search evaluation.
    /// It's purpose is to handle parallel evaluation,
    /// action generators creation based on information
    /// coming from bot instance.
    /// </summary>
    internal class MCTSEvaluationHandler : IDisposable
    {
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private MCTSTreeHandler[] treeHandlers;
        private readonly byte enemyPlayerId;

        public MCTSEvaluationHandler(PlayerPerspective playerPerspective,
                byte enemyPlayerId,
                Restrictions restrictions)
        {
            this.enemyPlayerId = enemyPlayerId;
            Initialize(playerPerspective, restrictions);
        }

        /// <summary>
        /// Initializes components for MCTS evaluation.
        /// </summary>
        /// <param name="initialGameState">Initial game state of the evaluation.</param>
        /// <param name="restrictions">Restrictions for the game.</param>
        /// <remarks>
        /// Must be called only if the evaluation is stopped, 
        /// otherwise it can cause unpredicted behaviour.
        /// </remarks>
        public void Initialize(PlayerPerspective initialGameState,
            Restrictions restrictions)
        {
            var distanceMatrix = new DistanceMatrix(initialGameState.MapMin.RegionsMin);
            ISuperRegionMinEvaluator gameBeginningSuperRegionMinEvaluator = new GameBeginningSuperRegionMinEvaluator(initialGameState.MapMin);
            IRegionMinEvaluator gameBeginningRegionMinEvaluator = new GameBeginningRegionMinEvaluator(gameBeginningSuperRegionMinEvaluator, distanceMatrix);
            ISuperRegionMinEvaluator gameSuperRegionMinEvaluator = new GameSuperRegionMinEvaluator(initialGameState.MapMin);
            IRegionMinEvaluator gameRegionMinEvaluator = new GameRegionMinEvaluator(gameSuperRegionMinEvaluator);

            IRoundEvaluator roundEvaluator = new RoundEvaluator();
            IPlayerPerspectiveEvaluator gameBeginningPlayerPerspectiveEvaluator = new PlayerPerspectiveEvaluator(gameBeginningRegionMinEvaluator);
            IPlayerPerspectiveEvaluator gamePlayerPerspectiveEvaluator = new PlayerPerspectiveEvaluator(
                gameRegionMinEvaluator);

            var selectRegionActionsGenerator = new SelectRegionActionsGenerator(gameBeginningRegionMinEvaluator, restrictions.GameBeginningRestrictions);
            var gameActionsGenerator = new MCTSBotActionsGenerator(gameRegionMinEvaluator, initialGameState.MapMin);

            // clear what was left after previous evaluation
            CancellationTokenSource = new CancellationTokenSource();
            ClearEvaluationCache();

            // # tree handlers == how much parallely do we want it to evaluate
            treeHandlers = new MCTSTreeHandler[1];

            for (int index = 0; index < treeHandlers.Length; index++)
            {
                treeHandlers[index] = new MCTSTreeHandler(initialGameState,
                    enemyPlayerId,
                    gameActionsGenerator,
                    selectRegionActionsGenerator,
                    gamePlayerPerspectiveEvaluator,
                    roundEvaluator);
            }
        }

        
        public BotTurn GetBestMove()
        {
            // TODO: merge the trees and get the most visited node
            return treeHandlers[0].Tree.Root.Children
                .OrderByDescending(x => x.VisitCount).ThenByDescending(x => x.WinCount).First().Value.BotTurn;
        }

        /// <summary>
        /// Asynchronously starts MCTS evaluation in parallel.
        /// </summary>
        /// <returns></returns>
        public async Task StartEvaluationAsync()
        {
            Task[] tasks = new Task[treeHandlers.Length];

            CancellationTokenSource = new CancellationTokenSource();

            for (int i = 0; i < tasks.Length; i++)
            {
                var treeHandler = treeHandlers[i];

                tasks[i] = Task.Factory.StartNew(() => treeHandler
                    .StartEvaluation(CancellationTokenSource.Token), CancellationToken.None,
                    TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Stops the evaluation.
        /// </summary>
        public void Stop()
        {
            CancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Asynchronously clears cache after evaluation.
        /// </summary>
        /// <returns></returns>
        public async Task ClearEvaluationCacheAsync()
        {
            Task[] tasks = new Task[treeHandlers.Length];

            for (int i = 0; i < tasks.Length; i++)
            {
                int i1 = i;
                tasks[i] = Task.Run(() => treeHandlers[i1].Tree.FreeEntireTree());
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Clears evaluation cache.
        /// </summary>
        public void ClearEvaluationCache()
        {
            if (treeHandlers != null)
            {
                foreach (var treeHandler in treeHandlers)
                {
                    treeHandler.Tree.FreeEntireTree();
                }
            }
        }

        /// <summary>
        /// Disposes resources, clearing evaluation cache.
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