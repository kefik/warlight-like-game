namespace GameAi.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using Common.Extensions;
    using Data;
    using Data.EvaluationStructures;
    using Data.Restrictions;
    using NUnit.Framework;

    using static NUnit.Framework.Assert;

    [TestFixture]
    public class GameBotCreatorTests
    {
        private GameBotCreator creator;
        private RegionMin[] regions;
        private SuperRegionMin[] superRegions;

        private byte pc1,
            pc2;

        [SetUp]
        public void Initialize()
        {
            creator = new GameBotCreator();

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

            regions = new[]
            {
                czechia, germany, poland, slovakia, austria
            };

            europe.RegionsIds = new[]
            {
                czechia.Id, germany.Id, poland.Id, slovakia.Id, austria.Id
            };

            superRegions = new[] { europe };
        }

        [Test]
        public void VisibilityTest()
        {
            var mapMin = new MapMin(regions, superRegions);

            // take pc1 as referenced player
            var playerPerspective = new PlayerPerspective(mapMin, pc1);
            mapMin = playerPerspective.MapMin;
            
            creator.InitializeVisibility(ref playerPerspective,
                isFogOfWar: true);

            // check visibility of the regions
            {
                ref var czech = ref mapMin.RegionsMin[0];
                ref var germany = ref mapMin.RegionsMin[1];
                ref var poland = ref mapMin.RegionsMin[2];
                ref var slovakia = ref mapMin.RegionsMin[3];
                ref var austria = ref mapMin.RegionsMin[4];

                IsTrue(czech.IsVisible);
                IsTrue(germany.IsVisible);
                IsTrue(slovakia.IsVisible);
                IsTrue(poland.IsVisible);
                IsFalse(austria.IsVisible);
            }

            // take pc2 as referenced player
            playerPerspective = new PlayerPerspective(mapMin, pc2);
            mapMin = playerPerspective.MapMin;

            creator.InitializeVisibility(ref playerPerspective,
                isFogOfWar: true);

            // check visibility of the regions
            {
                ref var czech = ref mapMin.RegionsMin[0];
                ref var germany = ref mapMin.RegionsMin[1];
                ref var poland = ref mapMin.RegionsMin[2];
                ref var slovakia = ref mapMin.RegionsMin[3];
                ref var austria = ref mapMin.RegionsMin[4];

                IsTrue(czech.IsVisible);
                IsTrue(germany.IsVisible);
                IsTrue(slovakia.IsVisible);
                IsFalse(poland.IsVisible);
                IsTrue(austria.IsVisible);
            }
        }
    }
}