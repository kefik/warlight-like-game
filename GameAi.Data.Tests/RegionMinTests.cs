namespace GameAi.Data.Tests
{
    using EvaluationStructures;
    using NUnit.Framework;

    using static NUnit.Framework.Assert;

    [TestFixture]
    public class RegionMinTests
    {
        private RegionMin regionMin;
        private byte player;

        [SetUp]
        public void SetUp()
        {
            regionMin = new RegionMin(1, 2, 2, 3) { NeighbourRegionsIds = new[] {2, 3, 4}};
        }

        [Test]
        public void ConstructTest()
        {
            Assert.AreEqual(2, regionMin.Army);

            Assert.IsFalse(regionMin.IsWasteland);

            Assert.AreEqual(3, regionMin.OwnerId);
        }

        [Test]
        public void AssignTest1()
        {
            regionMin.Army = 10;

            Assert.AreEqual(10, regionMin.Army);

            regionMin.IsVisible = true;

            Assert.IsTrue(regionMin.IsVisible);

            regionMin.OwnerId = 8;

            Assert.AreEqual(8, regionMin.OwnerId);
        }

        [Test]
        public void AssignTest2()
        {
            regionMin.OwnerId = 30;
            Assert.AreEqual(30, regionMin.OwnerId);
            AreEqual(2, regionMin.Army);
            IsFalse(regionMin.IsVisible);

            regionMin.OwnerId = 255;
            AreNotEqual(255, regionMin.OwnerId);
            AreEqual(2, regionMin.Army);
            IsFalse(regionMin.IsVisible);

            regionMin.OwnerId = 31;
            Assert.AreEqual(31, regionMin.OwnerId);
            AreEqual(2, regionMin.Army);
            IsFalse(regionMin.IsVisible);

            regionMin.IsVisible = true;
            AreEqual(31, regionMin.OwnerId);
            AreEqual(2, regionMin.Army);
            IsTrue(regionMin.IsVisible);

            regionMin.Army = 500;
            AreEqual(31, regionMin.OwnerId);
            AreEqual(500, regionMin.Army);
            IsTrue(regionMin.IsVisible);

            regionMin.Army = 0;
            AreEqual(31, regionMin.OwnerId);
            AreEqual(0, regionMin.Army);
            IsTrue(regionMin.IsVisible);

            regionMin.Army = 1023;
            AreEqual(31, regionMin.OwnerId);
            AreEqual(1023, regionMin.Army);
            IsTrue(regionMin.IsVisible);

            regionMin.IsVisible = false;
            AreEqual(31, regionMin.OwnerId);
            AreEqual(1023, regionMin.Army);
            IsFalse(regionMin.IsVisible);
        }

        [Test]
        public void NeighbourDetectionTest()
        {
            player = 50;

            Assert.IsTrue(regionMin.GetOwnerPerspective(player) == OwnerPerspective.Enemy);

            player = 3;

            Assert.IsTrue(regionMin.GetOwnerPerspective(player) == OwnerPerspective.Mine);
        }

        [Test]
        public void UnoccupiedTest()
        {
            regionMin.OwnerId = 0;

            Assert.IsTrue(regionMin.GetOwnerPerspective(player) == OwnerPerspective.Unoccupied);
        }
    }
}