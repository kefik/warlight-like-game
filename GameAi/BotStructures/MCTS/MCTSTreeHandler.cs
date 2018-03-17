namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators;
    using Interfaces.Evaluators.NodeEvaluators;

    /// <summary>
    /// Handles single-threaded MCTS evaluation.
    /// </summary>
    internal class MCTSTreeHandler
    {
        private readonly INodeEvaluator<MCTSTreeNode> nodeEvaluator;
        private readonly IGameActionsGenerator gameActionsGenerator;
        private readonly IGameBeginningActionsGenerator beginningActionsGenerator;
        private byte myPlayerId;
        private byte[] playersIds;

        /// <summary>
        /// Tree representing the evaluation.
        /// </summary>
        public MCTSTree Tree { get; }

        public MCTSTreeHandler(PlayerPerspective initialBoardState,
            byte[] playersIds,
            INodeEvaluator<MCTSTreeNode> nodeEvaluator,
            IRoundEvaluator roundEvaluator,
            IGameActionsGenerator gameActionsGenerator,
            IGameBeginningActionsGenerator beginningActionsGenerator)
        {
            var state = new NodeState()
            {
                BoardState = initialBoardState,
                VisitCount = 0,
                WinCount = 0
            };

            Tree = new MCTSTree(state);

            this.nodeEvaluator = nodeEvaluator;
            this.gameActionsGenerator = gameActionsGenerator;
            this.beginningActionsGenerator = beginningActionsGenerator;

            myPlayerId = initialBoardState.PlayerId;
            this.playersIds = playersIds;
        }

        /// <summary>
        /// Starts MCTS evaluation with possible cancellation.
        /// </summary>
        /// <param name="token"></param>
        public void StartEvaluation(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var bestNode = SelectBestNode();

                var nodesToSimulate = Expand(bestNode);

                Simulate(nodesToSimulate);

                Backpropagate(nodesToSimulate);
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
        /// Expand the node specified in parameter meaning choose nexta ctions that will be simulated.
        /// </summary>
        /// <param name="node">Node to expand.</param>
        /// <returns>New children <see cref="node"/> was expanded to.</returns>
        private IList<MCTSTreeNode> Expand(MCTSTreeNode node)
        {
            if (node.GameState.MapMin.IsGameBeginning())
            {
                var myBotTurn = beginningActionsGenerator.Generate(node.GameState);

                // TODO: try to generate best moves from opponents perspective (fog of war problem)
                // TODO: get gameState this action leads to
                foreach (byte playerId in playersIds.Where(x => x != myPlayerId))
                {
                    var playerPerspective = node.Value.BoardState;
                    playerPerspective.PlayerId = playerId;

                    var botTurn = beginningActionsGenerator.Generate(playerPerspective).First();
                }

                node.AddChild(new NodeState()
                {
                });
            }
            else
            {
                var botTurn = gameActionsGenerator.Generate(node.GameState);

                // TODO: get gameState this action leads to

                node.AddChild(new NodeState()
                {
                });
            }

            return node.Children;
        }

        private void Simulate(ICollection<MCTSTreeNode> nodes)
        {
            foreach (var node in nodes)
            {
                Simulate(node);
            }
        }

        private void Simulate(MCTSTreeNode sourceNode)
        {
            throw new NotImplementedException();
        }

        private void Backpropagate(IEnumerable<MCTSTreeNode> nodes)
        {
            foreach (MCTSTreeNode node in nodes)
            {
                Backpropagate(node);
            }
        }

        /// <summary>
        /// Backpropagates the changes back to the root.
        /// </summary>
        /// <param name="newlyExpandedNode"></param>
        private void Backpropagate(MCTSTreeNode newlyExpandedNode)
        {
            MCTSTreeNode currentNode = newlyExpandedNode.Parent;
            // repeat until the current node is root node
            do
            {
                // update the node
                currentNode.Value.VisitCount += newlyExpandedNode.VisitCount;
                currentNode.Value.WinCount += newlyExpandedNode.Value.WinCount;
            } while (!(currentNode = newlyExpandedNode.Parent).IsRoot);
        }
    }
}