namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EvaluationStructures;
    using GameObjectsLib.GameRecording;
    using GameRecording;
    using Interfaces;

    /// <summary>
    /// Component handling Monte Carlo tree search evaluation.
    /// </summary>
    internal class MCTSEvaluationHandler
    {
        internal CancellationTokenSource CancellationTokenSource { get; set; }
        private MCTSTreeHandler[] TreeHandlers { get; }

        public MCTSEvaluationHandler(PlayerPerspective initialGameState, INodeEvaluator<MCTSTreeNode> nodeEvaluator)
        {
            TreeHandlers = new MCTSTreeHandler[2];
            CancellationTokenSource = new CancellationTokenSource();

            for (int index = 0; index < TreeHandlers.Length; index++)
            {
                TreeHandlers[index] = new MCTSTreeHandler(initialGameState, nodeEvaluator);
            }
        }

        public BotTurn GetBestMove()
        {
            // TODO: merge the trees and get the most visitted node
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously starts MCTS evaluation.
        /// </summary>
        /// <returns></returns>
        public async Task StartEvaluationAsync()
        {
            Task[] tasks = new Task[TreeHandlers.Length];

            for (int i = 0; i < tasks.Length; i++)
            {
                var treeHandler = TreeHandlers[i];

                tasks[i] = Task.Run(() => treeHandler.StartEvaluation(CancellationTokenSource.Token));
                await tasks[i];
            }
            
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Stops the evaluation.
        /// </summary>
        public void Stop()
        {
            CancellationTokenSource.Cancel();
            CancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Asynchronously clears evaluation cache.
        /// </summary>
        /// <returns></returns>
        public async Task ClearEvaluationCacheAsync()
        {
            Task[] tasks = new Task[TreeHandlers.Length];

            for (int i = 0; i < tasks.Length; i++)
            {
                var treeHandler = TreeHandlers[i];

                tasks[i] = Task.Run(() => treeHandler.Tree.FreeEntireTree());
            }

            await Task.WhenAll(tasks);
        }
    }
}