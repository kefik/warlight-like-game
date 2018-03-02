namespace GameAi.BotStructures.MCTS
{
    using System;
    using Interfaces;

    internal class UctEvaluator : INodeEvaluator<MCTSTreeNode>
    {
        private const double Coefficient = 1.41;

        public double GetValue(MCTSTreeNode node)
        {
            if (node.Value.VisitCount == 0)
            {
                return double.MaxValue;
            }

            return ((double)node.Value.WinCount / node.Value.VisitCount) + Coefficient
                   + Math.Sqrt(Math.Log(node.Parent.VisitCount) / node.VisitCount);
        }
    }
}