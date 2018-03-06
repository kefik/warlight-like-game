namespace GameAi.Tests
{
    using Data.EvaluationStructures;
    using NUnit.Framework;
    using static NUnit.Framework.Assert;

    [TestFixture]
    public class PlayerPerspectiveTests
    {
        private PlayerPerspective playerPerspective;

        [SetUp]
        public void Initialize()
        {
            RegionMin[] regionsMin = {
                new RegionMin(0, 0, 2)
                {
                    NeighbourRegionsIds = new []{1}
                },
                new RegionMin(1, 0, 4, ownerIdPlayer: 1)
                {
                    NeighbourRegionsIds = new []{0}
                },
            };

            SuperRegionMin[] superRegionsMin =
            {
                new SuperRegionMin(0, 5)
                {
                    RegionsIds = new []{0, 1}
                }
            };

            playerPerspective = new PlayerPerspective(regionsMin, superRegionsMin, playerId: 1);
        }

        [Test]
        public void IsRegionMineTest()
        {
            var regions = playerPerspective.MapMin.RegionsMin;

            int regionId = regions[0].Id;
            IsFalse(playerPerspective.IsRegionMine(regionId));

            int otherRegionId = regions[1].Id;
            IsTrue(playerPerspective.IsRegionMine(otherRegionId));
        }

        [Test]
        public void IsNeighbourToAnyMyRegionTest()
        {
            var regions = playerPerspective.MapMin.RegionsMin;
            IsTrue(playerPerspective.IsNeighbourToAnyMyRegion(regions[0]));
            IsFalse(playerPerspective.IsNeighbourToAnyMyRegion(regions[1]));
        }
    }
}