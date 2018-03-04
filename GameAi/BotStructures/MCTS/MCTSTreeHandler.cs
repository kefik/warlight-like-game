namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EvaluationStructures;
    using GameRecording;
    using Interfaces;

    internal class MCTSTreeHandler
    {
        private readonly INodeEvaluator<MCTSTreeNode> nodeEvaluator;
        private readonly IGameActionGenerator<BotGameTurn, PlayerPerspective> actionGenerator;
        private readonly IGameActionGenerator<BotGameBeginningTurn, PlayerPerspective> beginningActionGenerator;
        
        public MCTSTree Tree { get; }

        public MCTSTreeHandler(PlayerPerspective initialBoardState, INodeEvaluator<MCTSTreeNode> nodeEvaluator,
            IGameActionGenerator<BotGameTurn, PlayerPerspective> actionGenerator,
            IGameActionGenerator<BotGameBeginningTurn, PlayerPerspective> beginningActionGenerator = null)
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

        public void StartEvaluation(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var bestNode = SelectBestNode();
                
                var action = actionGenerator.Generate(bestNode.GameState);
                throw new OperationCanceledException();
            }
        }

        public void StartGameBeginningEvaluation(CancellationToken token)
        {
            
        }

        private bool ShouldGenerateGameBeginningAction(MCTSTreeNode currentNode)
        {
            byte playerId = currentNode.Value.BoardState.PlayerId;

            if (currentNode.IsRoot)
            {
                return true;
            }

            do
            {
                currentNode = currentNode.Parent;
                if (currentNode.Value.BoardState.PlayerId == playerId)
                {
                    return true;
                }
            } while (!currentNode.IsRoot);

            return false;
        }
        
        /// <summary>
        /// Selects the best node.
        /// </summary>
        /// <returns></returns>
        private MCTSTreeNode SelectBestNode()
        {
            MCTSTreeNode currentNode = Tree.Root;

            if (currentNode == null)
            {
                throw new ArgumentException("The tree does not have a root.");
            }

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
        private void ExpandNode(MCTSTreeNode node)
        {
            throw new NotImplementedException();
        }
    }
}