namespace GameAi.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using Common.Extensions;
    using Data;
    using Data.EvaluationStructures;
    using Data.Restrictions;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;
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

            SuperRegionMin europe = new SuperRegionMin(1, 5);

            var czechia = new RegionMin(1, europe.Id, 2);
            var germany = new RegionMin(2, europe.Id, 2);
            var poland = new RegionMin(3, europe.Id, 2)
            {
                OwnerId = pc1
            };
            var slovakia = new RegionMin(4, europe.Id, 2);
            var austria = new RegionMin(5, europe.Id, 2)
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
        public void CreateMapForBotTest()
        {
            var mapMin = creator.CreateMapForBot(regions, superRegions,
                out var regionIdsMappingDictionary,
                out var superRegionsIdsMappingDictionary);

            // check regions
            // czechia
            AreEqual(0, regionIdsMappingDictionary.GetNewId(1));
            AreEqual(1, regionIdsMappingDictionary.GetOriginalId(0));

            // austria
            AreEqual(4, regionIdsMappingDictionary.GetNewId(5));
            AreEqual(5, regionIdsMappingDictionary.GetOriginalId(4));

            // check super regions
            AreEqual(0, superRegionsIdsMappingDictionary.GetNewId(1));
            AreEqual(1, superRegionsIdsMappingDictionary.GetOriginalId(0));

            // check for correct neighbours
            ref var poland = ref mapMin.RegionsMin[2];
            AreEqual(pc1, poland.OwnerId);
            Contains(0, poland.NeighbourRegionsIds);
            Contains(1, poland.NeighbourRegionsIds);
            Contains(3, poland.NeighbourRegionsIds);
        }

        [Test]
        public void VisibilityTest()
        {
            var mapMin = creator.CreateMapForBot(regions, superRegions,
                out var regionIdsMappingDictionary,
                out var superRegionsIdsMappingDictionary);

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