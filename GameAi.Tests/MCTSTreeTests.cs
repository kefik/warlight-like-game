namespace GameAi.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using BotStructures.MCTS;
    using Common.Collections;
    using NUnit.Framework;

    using static NUnit.Framework.Assert;
    using static Common.ObjectPool<BotStructures.MCTS.MCTSTreeNode>;

    [Ignore("Not working now")]
    public class MCTSTreeTests
    {
        private MCTSTree tree;

        [SetUp]
        public void Initialize()
        {
            //tree = new MCTSTree();

            //var root = new MCTSTreeNode();

            //var left = new MCTSTreeNode()
            //{
            //    Parent = root,
            //    Value = new PlayerPerspective()
            //};

            //var right = new MCTSTreeNode()
            //{
            //    Parent = root,
            //    Value = new PlayerPerspective()
            //};

            //root.Children = new List<MCTSTreeNode>()
            //{
            //    left, right
            //};

            //tree.Root = root;
        }
        
        [Test]
        public void AddChildTest()
        {
            var root = tree.Root;

            //root.AddChild(new PlayerPerspective(new RegionMin[1], new SuperRegionMin[1], 1));
            //root.AddChild(new PlayerPerspective(new RegionMin[2], new SuperRegionMin[2], 2));

            AreEqual(root.Children.Count, 4);

            var lastButOne = root.Children[2];
            var last = root.Children[3];

            IsTrue(lastButOne.IsLeaf);
            IsTrue(last.IsLeaf);

            AreSame(lastButOne.Parent, root);
        }

        [Test]
        public void FreeTreeTest()
        {
            var root = tree.Root;

            tree.FreeEntireTree();

            IsNull(root.Children);
            IsNull(tree.Root);
        }
    }
}