namespace GameAi.Tests
{
    using Data;
    using Data.EvaluationStructures;
    using NUnit.Framework;

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

            regionMin.IsVisible = true;

            Assert.IsTrue(regionMin.IsVisible);

            regionMin.Army = 500;

            Assert.AreEqual(500, regionMin.Army);
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