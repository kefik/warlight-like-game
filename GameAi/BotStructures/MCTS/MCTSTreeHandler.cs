﻿namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EvaluationStructures;
    using Interfaces;
    using InterFormatCommunication.GameRecording;

    internal class MCTSTreeHandler
    {
        private readonly INodeEvaluator<MCTSTreeNode> nodeEvaluator;
        private readonly IGameActionGenerator<BotGameTurn, PlayerPerspective> actionGenerator;
        private readonly IGameActionGenerator<BotGameBeginningTurn, PlayerPerspective> beginningActionGenerator;
        
        public MCTSTree Tree { get; }

        public MCTSTreeHandler(PlayerPerspective initialBoardState, INodeEvaluator<MCTSTreeNode> nodeEvaluator,
            IGameActionGenerator<BotGameBeginningTurn, PlayerPerspective> beginningActionGenerator,
            IGameActionGenerator<BotGameTurn, PlayerPerspective> actionGenerator)
        {
            var state = new NodeState()
            {
                BoardState = initialBoardState,
                VisitCount = 0,
                WinCount =  0
            };

            Tree = new MCTSTree(state);

            this.nodeEvaluator = nodeEvaluator;
            this.actionGenerator = actionGenerator;
            this.beginningActionGenerator = beginningActionGenerator;
        }

        /// <summary>
        /// Starts evaluation.
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
        private IList<MCTSTreeNode> Expand(MCTSTreeNode node)
        {
            for (int i = 0; i < 10; i++)
            {
                var botTurn = beginningActionGenerator.Generate(node.GameState);

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

        private void Backpropagate(ICollection<MCTSTreeNode> nodes)
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