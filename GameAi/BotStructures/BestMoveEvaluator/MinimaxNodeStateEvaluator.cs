namespace GameAi.BotStructures.BestMoveEvaluator
{
    using Common.Collections;
    using Data.EvaluationStructures;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.StructureEvaluators;
    using MCTS;

    internal class MinimaxTreeNodeState
    {
        public double Value { get; set; }
        public MapMin BoardState { get; set; }
    }

    internal class MinimaxTreeNode : TreeNode<MinimaxTreeNode, int>
    {
        
    }

    internal class MinimaxNodeStateEvaluator
    {
        private byte myPlayerId;
        private byte enemyPlayerId;
        private int maxDepth;

        private IPlayerPerspectiveEvaluator playerPerspectiveEvaluator;
        private IGameActionsGenerator actionsGenerator;

        public double? Result { get; set; }

        public MinimaxNodeStateEvaluator(
            IPlayerPerspectiveEvaluator playerPerspectiveEvaluator,
            byte enemyPlayerId, byte myPlayerId, int maxDepth)
        {
            this.playerPerspectiveEvaluator = playerPerspectiveEvaluator;
            this.enemyPlayerId = enemyPlayerId;
            this.myPlayerId = myPlayerId;
            this.maxDepth = maxDepth;
        }

        public void Evaluate(NodeState nodeState)
        {
            // TODO: alpha beta pruning minimax
        }
    }
}