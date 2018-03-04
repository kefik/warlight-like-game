namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EvaluationStructures;
    using GameObjectsLib.GameRecording;
    using Interfaces;
    using InterFormatCommunication.GameRecording;

    /// <summary>
    /// Component handling Monte Carlo tree search evaluation.
    /// </summary>
    internal class MCTSEvaluationHandler : IDisposable
    {
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private MCTSTreeHandler[] TreeHandlers { get; }

        public MCTSEvaluationHandler(PlayerPerspective initialGameState, INodeEvaluator<MCTSTreeNode> nodeEvaluator,
                IGameActionGenerator<BotGameBeginningTurn, PlayerPerspective> beginningGameActionGenerator,
                params IGameActionGenerator<BotGameTurn, PlayerPerspective>[] actionGenerators)
        {
            TreeHandlers = new MCTSTreeHandler[1];
            CancellationTokenSource = new CancellationTokenSource();

            for (int index = 0; index < TreeHandlers.Length; index++)
            {
                TreeHandlers[index] = new MCTSTreeHandler(initialGameState, nodeEvaluator, beginningGameActionGenerator, actionGenerators[0]);
            }
        }

        public BotTurn GetBestMove()
        {
            // TODO: merge the trees and get the most visited node
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
        /// Disposes resources, clearing evaluation cache.
        /// </summary>
        public void Dispose()
        {
            foreach (var treeHandler in TreeHandlers)
            {
                treeHandler.Tree.FreeEntireTree();
            }
        }

        ~MCTSEvaluationHandler()
        {
            Dispose();
        }
    }
}