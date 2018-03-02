namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EvaluationStructures;
    using Interfaces;

    internal class MCTSTreeHandler
    {
        private readonly INodeEvaluator<MCTSTreeNode> nodeEvaluator;
        private CancellationToken token;

        public MCTSTree Tree { get; }

        public MCTSTreeHandler(PlayerPerspective initialBoardState, INodeEvaluator<MCTSTreeNode> nodeEvaluator)
        {
            var state = new NodeState()
            {
                BoardState = initialBoardState,
                VisitCount = 0,
                WinCount =  0
            };

            Tree = new MCTSTree(state);

            this.nodeEvaluator = nodeEvaluator;
        }

        public void StartEvaluation(CancellationToken token)
        {
            this.token = token;

            while (!token.IsCancellationRequested)
            {
                var bestNode = SelectBestNode();

                throw new OperationCanceledException();
            }
        }

        private void StartEvaluation()
        {
            while (true)
            {
                var bestNode = SelectBestNode();

                throw new OperationCanceledException();
            }
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