﻿namespace GameAi.BotStructures.MCTS
{
    using System;
    using Common;
    using Common.Collections;
    using EvaluationStructures;
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
            Root = new MCTSTreeNode()
            {
                Value = nodeState
            };
        }

        /// <summary>
        /// Frees the specified node, returning it to the pool.
        /// </summary>
        /// <param name="node"></param>
        /// <remarks>Works weirdly if called on non-leaf node.</remarks>
        public void FreeNode(MCTSTreeNode node)
        {
            node.Children = null;
            node.Parent = null;
            node.Value = default(NodeState);

            DefaultPool.Free(node);
        }

        /// <summary>
        /// Frees the allocated tree.
        /// </summary>
        public void FreeEntireTree()
        {
            // free every node
            ForEachPreOrder(FreeNode);
        }
    }

    /// <summary>
    /// One node of <see cref="MCTSTree"/>.
    /// </summary>
    internal class MCTSTreeNode : TreeNode<MCTSTreeNode, NodeState>
    {
        public int VisitCount
        {
            get { return Value.VisitCount; }
        }

        public override void AddChild(NodeState value)
        {
            var node = DefaultPool.Allocate();

            node.Value = value;
            node.Parent = this;

            Children.Add(node);
        }
    }
}