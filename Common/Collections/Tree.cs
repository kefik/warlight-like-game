namespace Common.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a general tree structure with n children at each level.
    /// </summary>
    /// <typeparam name="TValue">Type of the node value.</typeparam>
    /// <typeparam name="TNodeType"></typeparam>
    public class Tree<TNodeType, TValue> where TNodeType : TreeNode<TNodeType, TValue>, new()
    {
        /// <summary>
        /// Root of the tree. Its parent is null.
        /// </summary>
        public TNodeType Root { get; set; }

        /// <summary>
        /// Traverses the tree post-order and does specified action on each tree node.
        /// The specified action is invoked after every child node is visited.
        /// </summary>
        /// <param name="action">Action to be invoked on every node.</param>
        public void ForEachPostOrder(Action<TNodeType> action)
        {
            if (Root != null)
            {
                ForEachPostOrder(Root, action);
            }
        }

        private void ForEachPostOrder(TNodeType currentNode, Action<TNodeType> action)
        {
            if (!currentNode.IsLeaf)
            {
                foreach (var currentNodeChild in currentNode.Children)
                {
                    ForEachPostOrder(currentNodeChild, action);
                }
            }

            action(currentNode);
        }

        /// <summary>
        /// Traverses the tree pre-order and does specified action on each tree node.
        /// The specified action is invoked after every child node is visited.
        /// </summary>
        /// <param name="action">Action to be invoked on every node.</param>
        public void ForEachPreOrder(Action<TNodeType> action)
        {
            if (Root != null)
            {
                ForEachPreOrder(Root, action);
            }
        }

        private void ForEachPreOrder(TNodeType currentNode, Action<TNodeType> action)
        {
            // save children (handling situation when action resets currentNode's children)
            var children = currentNode.Children;

            action(currentNode);

            if (!currentNode.IsLeaf)
            {
                foreach (var currentNodeChild in children)
                {
                    ForEachPreOrder(currentNodeChild, action);
                }
            }
        }
    }

    /// <summary>
    /// Represents a tree node.
    /// </summary>
    /// <typeparam name="TValue">Value that the node contains.</typeparam>
    /// <typeparam name="TNodeType">Type of the node.</typeparam>
    public abstract class TreeNode<TNodeType, TValue> where TNodeType : TreeNode<TNodeType, TValue>, new()
    {
        public TNodeType Parent { get; set; }
        public List<TNodeType> Children { get; set; }

        public TValue Value { get; set; }

        public bool IsLeaf
        {
            get { return Children == null || Children.Count == 0; }
        }

        public bool IsRoot
        {
            get { return Parent == null; }
        }

        public TNodeType GetChild(int index)
        {
            return Children[index];
        }

        public virtual void AddChild(TValue value)
        {
            var node = new TNodeType()
            {
                Value = value,
                Parent = (TNodeType)this
            };

            Children.Add(node);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}