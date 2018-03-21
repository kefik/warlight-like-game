//#define IGNORE_CANCELLATION

namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.NodeEvaluators;
    using Interfaces.Evaluators.StructureEvaluators;
    using StructuresEvaluators;

    /// <summary>
    /// Handles single-threaded MCTS evaluation.
    /// </summary>
    internal class MCTSTreeHandler
    {
        private readonly INodeEvaluator<MCTSTreeNode> nodeEvaluator;
        private readonly IGameActionsGenerator gameActionsGenerator;
        private readonly IGameBeginningActionsGenerator beginningActionsGenerator;
        private readonly IPlayerPerspectiveEvaluator gamePlayerPerspectiveEvaluator;

        private byte myPlayerId;
        private byte enemyPlayerId;

        private Random random;

        /// <summary>
        /// Tree representing the evaluation.
        /// </summary>
        public MCTSTree Tree { get; }

        public MCTSTreeHandler(PlayerPerspective initialBoardState,
            byte enemyPlayerId,
            IGameActionsGenerator gameActionsGenerator,
            IGameBeginningActionsGenerator beginningActionsGenerator,
            IPlayerPerspectiveEvaluator gamePlayerPerspectiveEvaluator)
        {
            var state = new NodeState()
            {
                BoardState = initialBoardState.MapMin,
                VisitCount = 0,
                WinCount = 0
            };

            Tree = new MCTSTree(state);

            this.nodeEvaluator = new UctEvaluator(Tree.Root);
            this.gameActionsGenerator = gameActionsGenerator;
            this.beginningActionsGenerator = beginningActionsGenerator;
            this.gamePlayerPerspectiveEvaluator = gamePlayerPerspectiveEvaluator;

            myPlayerId = initialBoardState.PlayerId;
            this.enemyPlayerId = enemyPlayerId;

            this.random = new Random();
        }

        /// <summary>
        /// Starts MCTS evaluation with possible cancellation.
        /// </summary>
        /// <param name="token"></param>
        public void StartEvaluation(CancellationToken token)
        {
#if IGNORE_CANCELLATION
            while (true)
#else
            while (!token.IsCancellationRequested)
#endif
            {
                var bestNode = SelectBestNode();

                var nodesToSimulate = Expand(bestNode);

                if (nodesToSimulate == null)
                {
                    continue;
                }

                var nodeToSimulate = nodesToSimulate[random.Next(nodesToSimulate.Count)];

                Simulate(nodeToSimulate);

                Backpropagate(nodeToSimulate);
            }
        }

        /// <summary>
        /// Selects the best node.
        /// </summary>
        /// <returns></returns>
        private MCTSTreeNode SelectBestNode()
        {
            MCTSTreeNode currentNode = Tree.Root;

            while (!currentNode.IsLeaf)
            {
                currentNode = GetBestChild(currentNode);
            }

            return currentNode;
        }

        private MCTSTreeNode GetBestChild(MCTSTreeNode node)
        {
            var children = node.Children;

            MCTSTreeNode bestChild = children[0];
            double bestChildValue = nodeEvaluator.GetValue(bestChild);

            // get the node with best rating
            for (int index = 1; index < children.Count; index++)
            {
                var child = children[index];
                double childValue = nodeEvaluator.GetValue(child);

                if (childValue > bestChildValue)
                {
                    bestChild = child;
                    bestChildValue = childValue;
                }
            }

            return bestChild;
        }

        /// <summary>
        /// Expand the node specified in parameter meaning choose next actions that will be simulated.
        /// </summary>
        /// <param name="node">Node to expand.</param>
        /// <returns>New children <see cref="node"/> was expanded to.</returns>
        private IList<MCTSTreeNode> Expand(MCTSTreeNode node)
        {
            var boardState = node.Value.BoardState;

            if (node.GameState.IsGameBeginning())
            {
               
            }
            else
            {
                
            }

            return node.Children;
        }

        /// <summary>
        /// Play a simulation from specified node.
        /// </summary>
        /// <param name="sourceNode"></param>
        private void Simulate(MCTSTreeNode sourceNode)
        {
            // TODO: minimax simulation
        }

        /// <summary>
        /// Backpropagates the information about results
        /// from newly expanded node back to root.
        /// </summary>
        /// <param name="newlyExpandedNode"></param>
        private void Backpropagate(MCTSTreeNode newlyExpandedNode)
        {
            MCTSTreeNode currentNode = newlyExpandedNode;
            // repeat until the current node is root node
            while (!currentNode.IsRoot)
            {
                MCTSTreeNode parent = currentNode.Parent;

                // update the node
                parent.Value.VisitCount += currentNode.VisitCount;
                parent.Value.WinCount += currentNode.WinCount;

                currentNode = currentNode.Parent;
            }
        }
    }
}