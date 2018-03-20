namespace GameAi.BotStructures.MCTS
{
    using System;
    using Interfaces;
    using Interfaces.Evaluators.NodeEvaluators;

    internal class UctEvaluator : INodeEvaluator<MCTSTreeNode>
    {
        private const double Coefficient = 1.41;

        private readonly MCTSTreeNode root;

        public UctEvaluator(MCTSTreeNode root)
        {
            this.root = root;
        }
        
        public double GetValue(MCTSTreeNode node)
        {
            if (node.Value.VisitCount == 0)
            {
                return double.MaxValue;
            }

            return node.WinCount / node.VisitCount + Coefficient
                   * Math.Sqrt(Math.Log(root.VisitCount) / node.VisitCount);
        }
    }
}