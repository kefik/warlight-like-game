namespace GameAi.Data.Tests
{
    using EvaluationStructures;
    using NUnit.Framework;

    [TestFixture]
    public class SuperRegionMinTests
    {
        private SuperRegionMin superRegionMin;

        [SetUp]
        public void SetUp()
        {
            superRegionMin = new SuperRegionMin(superRegionId: 20, bonusArmy: 6, owningPlayer: 100);
        }

        [Test]
        public void ConstructTest()
        {
            Assert.AreEqual(100, superRegionMin.OwnerId);
        }

        [Test]
        public void AssignTest()
        {
            superRegionMin.OwnerId = 55;

            Assert.AreEqual(55, superRegionMin.OwnerId);
        }
    }
}