namespace GameAi.Data.Tests
{
    using System.Linq;
    using EvaluationStructures;
    using NUnit.Framework;

    using static NUnit.Framework.Assert;

    [TestFixture]
    public class PlayerPerspectiveTests
    {
        private PlayerPerspective playerPerspective;
        private byte pc1, pc2;

        [SetUp]
        public void Initialize()
        {
            pc1 = 1;
            pc2 = 2;

            SuperRegionMin europe = new SuperRegionMin(0, 5);

            var czechia = new RegionMin(0, europe.Id, 2);
            var germany = new RegionMin(1, europe.Id, 2);
            var poland = new RegionMin(2, europe.Id, 2)
            {
                OwnerId = pc1
            };
            var slovakia = new RegionMin(3, europe.Id, 2);
            var austria = new RegionMin(4, europe.Id, 2)
            {
                OwnerId = pc2
            };

            czechia.NeighbourRegionsIds = new[]
            {
                germany.Id,
                poland.Id,
                slovakia.Id,
                austria.Id
            };
            germany.NeighbourRegionsIds = new[]
            {
                czechia.Id,
                austria.Id,
                poland.Id
            };
            poland.NeighbourRegionsIds = new[]
            {
                czechia.Id,
                germany.Id,
                slovakia.Id
            };
            slovakia.NeighbourRegionsIds = new[]
            {
                czechia.Id,
                poland.Id,
                austria.Id
            };
            austria.NeighbourRegionsIds = new[]
            {
                czechia.Id,
                germany.Id,
                slovakia.Id
            };

            var regions = new[]
            {
                czechia, germany, poland, slovakia, austria
            };

            europe.RegionsIds = new[]
            {
                czechia.Id, germany.Id, poland.Id, slovakia.Id, austria.Id
            };

            var superRegions = new[] { europe };

            playerPerspective = new PlayerPerspective(regions, superRegions, playerId: pc1);
        }

        [Test]
        public void IsRegionMineTest()
        {
            var regions = playerPerspective.MapMin.RegionsMin;

            int regionId = regions[0].Id;
            IsFalse(playerPerspective.IsRegionMine(regionId));

            int otherRegionId = regions[1].Id;
            IsFalse(playerPerspective.IsRegionMine(otherRegionId));
            
            IsTrue(playerPerspective.IsRegionMine(regions[2].Id));

            IsFalse(playerPerspective.IsRegionMine(regions[3].Id));

            IsFalse(playerPerspective.IsRegionMine(regions[4].Id));
        }

        [Test]
        public void IsNeighbourToAnyMyRegionTest()
        {
            var regions = playerPerspective.MapMin.RegionsMin;
            IsTrue(playerPerspective.IsNeighbourToAnyMyRegion(regions[0]));
            IsTrue(playerPerspective.IsNeighbourToAnyMyRegion(regions[1]));
            IsFalse(playerPerspective.IsNeighbourToAnyMyRegion(regions[2]));
            IsTrue(playerPerspective.IsNeighbourToAnyMyRegion(regions[3]));
            IsFalse(playerPerspective.IsNeighbourToAnyMyRegion(regions[4]));
        }

        [Test]
        public void GetMyRegionsTest()
        {
            var myRegions = playerPerspective
                .GetMyRegions().Select(x => x.Id).ToList();

            Contains(2, myRegions);
            AreEqual(1, myRegions.Count);
        }

        [Test]
        public void GetRegionTest()
        {
            var austria = playerPerspective.GetRegion(4);
            AreEqual(4, austria.Id);
            AreEqual(pc2, austria.OwnerId);

            var czechia = playerPerspective.GetRegion(0);
            AreEqual(0, czechia.Id);
            AreEqual(0, czechia.OwnerId);
        }

        [Test]
        public void GetNeighbourRegionsTest()
        {
            ref var czech = ref playerPerspective.MapMin.RegionsMin[0];
            ref var germany = ref playerPerspective.MapMin.RegionsMin[1];
            ref var poland = ref playerPerspective.MapMin.RegionsMin[2];
            ref var slovakia = ref playerPerspective.MapMin.RegionsMin[3];
            ref var austria = ref playerPerspective.MapMin.RegionsMin[4];

            // austria neighbours check
            var neighbourRegions = playerPerspective
                .GetNeighbourRegions(austria.Id).Select(x => x.Id).ToList();
            Contains(czech.OwnerId, neighbourRegions);
            Contains(germany.OwnerId, neighbourRegions);
            Contains(slovakia.OwnerId, neighbourRegions);
            AreEqual(3, neighbourRegions.Count);
        }
    }
}