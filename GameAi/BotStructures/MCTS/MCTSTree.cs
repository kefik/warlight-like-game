namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.Collections;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces;
    using static Common.ObjectPool<MCTSTreeNode>;

    
    /// <summary>
    /// Tree representing MCTS evaluation.
    /// </summary>
    /// <remarks>
    /// Every node must be of type <see cref="MCTSTreeNode"/>.
    /// </remarks>
    internal class MCTSTree : Tree<MCTSTreeNode, NodeState>
    {
        public MCTSTree(NodeState nodeState)
        {
            Root = DefaultPool.Allocate();
            Root.Value = nodeState;
        }

        /// <summary>
        /// Frees the specified node, returning it to the pool.
        /// </summary>
        /// <param name="node"></param>
        /// <remarks>Works weirdly if called on non-leaf node.</remarks>
        public void FreeNode(MCTSTreeNode node)
        {
            DefaultPool.Free(node);
        }

        /// <summary>
        /// Frees the allocated tree.
        /// </summary>
        public void FreeEntireTree()
        {
            // free every node
            ForEachPreOrder(FreeNode);

            // root freed => no reason to have pointer on it
            Root = null;
        }
    }

    /// <summary>
    /// One node of <see cref="MCTSTree"/>.
    /// </summary>
    internal class MCTSTreeNode : TreeNode<MCTSTreeNode, NodeState>
    {
        public MCTSTreeNode()
        {
            Children = new List<MCTSTreeNode>();
        }

        public int VisitCount
        {
            get { return Value.VisitCount; }
        }

        public double WinCount
        {
            get { return Value.WinCount; }
        }

        public MapMin GameState
        {
            get { return Value.BoardState; }
        }

        public BotTurn Action
        {
            get { return Value.BotTurn; }
        }

        public override MCTSTreeNode AddChild(NodeState value)
        {
            var node = DefaultPool.Allocate();

            node.Value = value;
            node.Parent = this;

            Children.Add(node);

            return node;
        }
    }
}