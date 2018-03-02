namespace Common.Tests.Collections
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Common.Collections;
    using NUnit.Framework;

    using static NUnit.Framework.Assert;

    internal class TestTreeNode : TreeNode<TestTreeNode, char>
    {
        
    }

    [TestFixture]
    public class TreeTests
    {
        private Tree<TestTreeNode, char> tree;

        [SetUp]
        public void Initialize()
        {
            tree = new Tree<TestTreeNode, char>();

            var b = new TestTreeNode();

            tree.Root = b;

            b.Value = 'B';

            var a = new TestTreeNode()
            {
                Parent = b,
                Value = 'A'
            };
            var d = new TestTreeNode()
            {
                Parent = b,
                Value = 'D'
            };

            b.Children = new List<TestTreeNode>()
            {
                a, d
            };

            var c = new TestTreeNode()
            {
                Parent = d,
                Value = 'C'
            };

            var e = new TestTreeNode()
            {
                Parent = d,
                Value = 'E'
            };

            d.Children = new List<TestTreeNode>()
            {
                c, e
            };
        }

        [Test]
        public void ForEachPostOrderTest()
        {
            var sb = new StringBuilder();

            tree.ForEachPostOrder(x => sb.Append($"{x.Value} "));

            AreEqual("A C E D B ", sb.ToString());
        }

        [Test]
        public void ForEachPreOrderTest()
        {
            var sb = new StringBuilder();

            tree.ForEachPreOrder(x => sb.Append($"{x.Value} "));

            AreEqual("B A D C E ", sb.ToString());
        }

        [Test]
        public void AddChildTest()
        {
            var d = tree.Root.Children[1];
            d.AddChild('F');

            var f = d.Children.Last();

            AreSame(f.Parent, d);
            AreEqual('F', f.Value);
        }
    }
}