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
    using Interfaces.Evaluators;
    using Interfaces.Evaluators.NodeEvaluators;
    using Interfaces.Evaluators.StructureEvaluators;
    using StructuresEvaluators;

    /// <summary>
    /// Handles single-threaded MCTS evaluation.
    /// </summary>
    internal class MCTSTreeHandler
    {
        private readonly INodeEvaluator<MCTSTreeNode> nodeEvaluator;
        private readonly IRoundEvaluator roundEvaluator;
        private readonly IGameActionsGenerator gameActionsGenerator;
        private readonly IGameBeginningActionsGenerator beginningActionsGenerator;
        private readonly IPlayerPerspectiveEvaluator gameBeginningPlayerPerspectiveEvaluator;
        private readonly IPlayerPerspectiveEvaluator gamePerspectiveEvaluator;

        private MinimaxCalculator minimaxCalculator;

        private byte myPlayerId;
        private byte enemyPlayerId;

        /// <summary>
        /// Tree representing the evaluation.
        /// </summary>
        public MCTSTree Tree { get; }

        public MCTSTreeHandler(PlayerPerspective initialBoardState,
            byte enemyPlayerId,
            INodeEvaluator<MCTSTreeNode> nodeEvaluator,
            IRoundEvaluator roundEvaluator,
            IGameActionsGenerator gameActionsGenerator,
            IGameBeginningActionsGenerator beginningActionsGenerator,
            IPlayerPerspectiveEvaluator gameBeginningPlayerPerspectiveEvaluator,
            IPlayerPerspectiveEvaluator gamePerspectiveEvaluator)
        {
            var state = new NodeState()
            {
                BoardState = initialBoardState.MapMin,
                VisitCount = 0,
                WinCount = 0
            };

            Tree = new MCTSTree(state);

            this.nodeEvaluator = nodeEvaluator;
            this.roundEvaluator = roundEvaluator;
            this.gameActionsGenerator = gameActionsGenerator;
            this.beginningActionsGenerator = beginningActionsGenerator;
            this.gameBeginningPlayerPerspectiveEvaluator = gameBeginningPlayerPerspectiveEvaluator;
            this.gamePerspectiveEvaluator = gamePerspectiveEvaluator;

            minimaxCalculator = new MinimaxCalculator(roundEvaluator,
                gameBeginningPlayerPerspectiveEvaluator);

            myPlayerId = initialBoardState.PlayerId;
            this.enemyPlayerId = enemyPlayerId;
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
            var boardState = node.Value.BoardState;

            if (node.GameState.IsGameBeginning())
            {
                var playerPerspective = new PlayerPerspective(boardState, myPlayerId);
                // generate select moves for my bot
                var myBotTurns = beginningActionsGenerator.Generate(playerPerspective);

                playerPerspective.PlayerId = enemyPlayerId;

                // generate select moves for enemy bot
                var enemyBotTurns = beginningActionsGenerator.Generate(playerPerspective);

                var expandedNodeStates = minimaxCalculator.CalculateBestActions(
                    boardState,
                    myPlayerId, enemyPlayerId,
                    myBotTurns, enemyBotTurns);

                // add expanded node states
                foreach (var nodeState in expandedNodeStates)
                {
                    node.AddChild(nodeState);
                }
            }
            else
            {
                var playerPerspective = new PlayerPerspective(boardState, myPlayerId);
                // generate select moves for my bot
                var myBotTurns = gameActionsGenerator.Generate(playerPerspective);

                playerPerspective.PlayerId = enemyPlayerId;

                // generate select moves for enemy bot
                var enemyBotTurns = gameActionsGenerator.Generate(playerPerspective);

                var expandedNodeStates = minimaxCalculator.CalculateBestActions(
                    boardState,
                    myPlayerId, enemyPlayerId,
                    myBotTurns, enemyBotTurns);

                // add expanded node states
                foreach (var nodeState in expandedNodeStates)
                {
                    node.AddChild(nodeState);
                }
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