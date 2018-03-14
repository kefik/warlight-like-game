namespace FormatConverters.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using Common.Extensions;
    using GameAi;
    using GameAi.Data;
    using GameAi.Data.Restrictions;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameRestrictions;
    using GameObjectsLib.GameUser;
    using GameObjectsLib.Players;
    using NUnit.Framework;
    using Region = GameObjectsLib.GameMap.Region;

    using static NUnit.Framework.Assert;

    [TestFixture]
    public class RestrictionsFormatConverterExtensionsTests
    {
        private GameObjectsRestrictions gameObjectsRestrictions;
        private Region czechia;
        private Region poland;
        private Region austria;
        private Region germany;

        private Player pc1;
        private Player testUser;

        private Map map;

        [SetUp]
        public void Initialize()
        {
            var beginningRestrictions = new List<GameObjectsBeginningRestriction>();

            // set current directory to be the test bin directory (resharper using its own)
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var mapXmlString = TestMaps.TestMap;

            using (var stream = mapXmlString.ToStream())
            {
                map = new Map(1, "TEST", 1, stream);
            }
            czechia = map.Regions.First(x => x.Name == "Czechia");
            poland = map.Regions.First(x => x.Name == "Poland");
            austria = map.Regions.First(x => x.Name == "Austria");
            germany = map.Regions.First(x => x.Name == "Germany");

            pc1 = new AiPlayer(Difficulty.Hard, "PC1", KnownColor.Beige, GameBotType.MonteCarloTreeSearchBot);
            testUser = new HumanPlayer(new LocalUser("TestUser"), KnownColor.ActiveBorder);
            
            beginningRestrictions.Add(new GameObjectsBeginningRestriction()
            {
                Player = pc1,
                RegionsPlayersCanChoose = { czechia, poland },
                RegionsToChooseCount = 2
            });
            beginningRestrictions.Add(new GameObjectsBeginningRestriction()
            {
                Player = testUser,
                RegionsPlayersCanChoose = { austria, germany },
                RegionsToChooseCount = 2
            });

            gameObjectsRestrictions = new GameObjectsRestrictions()
            {
                GameBeginningRestrictions = beginningRestrictions
            };
        }

        [Test]
        public void ToRestrictionsTest()
        {
            var restrictions = gameObjectsRestrictions.ToRestrictions();

            var beginningRestrictions = restrictions.GameBeginningRestrictions;

            AreEqual(2, beginningRestrictions.Count);

            // check player ids
            var playerIdsSelected =
                beginningRestrictions.Select(x => x.PlayerId).ToList();
            Contains(pc1.Id, playerIdsSelected);
            Contains(testUser.Id, playerIdsSelected);

            // check regions have no intersect
            var selectedRegions = (from restriction in beginningRestrictions
                                   from regionId in restriction.RestrictedRegions
                                   select regionId).ToList();
            That(selectedRegions, Is.EquivalentTo(selectedRegions.Distinct()));

            // check that regions are valid
            foreach (int selectedRegion in selectedRegions)
            {
                Contains(selectedRegion, new[]
                {
                    czechia.Id,
                    poland.Id,
                    austria.Id,
                    germany.Id
                });
            }

            // chose same number of regions for each
            AreEqual(1, beginningRestrictions
                .Select(x => x.RegionsPlayerCanChooseCount)
                .Distinct()
                .Count());
        }

        [Test]
        public void ToRemappedRestrictions()
        {
            var idsMappingDictionary = new IdsMappingDictionary();
            idsMappingDictionary.GetMappedIdOrInsert(0);
            idsMappingDictionary.GetMappedIdOrInsert(pc1.Id);
            idsMappingDictionary.GetMappedIdOrInsert(testUser.Id);

            var restrictions = gameObjectsRestrictions.ToRestrictions()
                .ToRemappedRestrictions(idsMappingDictionary);

            var beginningRestrictions = restrictions.GameBeginningRestrictions;


            AreEqual(2, beginningRestrictions.Count);

            // check player ids
            var playerIdsSelected =
                beginningRestrictions.Select(x => x.PlayerId).ToList();
            Contains(1, playerIdsSelected);
            Contains(2, playerIdsSelected);

            // check regions have no intersect
            var selectedRegions = (from restriction in beginningRestrictions
                                   from regionId in restriction.RestrictedRegions
                                   select regionId).ToList();
            That(selectedRegions, Is.EquivalentTo(selectedRegions.Distinct()));

            // check that regions are valid
            foreach (int selectedRegion in selectedRegions)
            {
                Contains(selectedRegion, new[]
                {
                    czechia.Id,
                    poland.Id,
                    austria.Id,
                    germany.Id
                });
            }

            // chose same number of regions for each
            AreEqual(1, beginningRestrictions
                .Select(x => x.RegionsPlayerCanChooseCount)
                .Distinct()
                .Count());
        }

        [Ignore("TODO")]
        public void ToGameObjectsRestrictions()
        {
            var restrictions = gameObjectsRestrictions.ToRestrictions();

            restrictions.ToGameRestrictions(map, new List<Player>()
            {
                pc1,
                testUser
            });

            // TODO
        }
    }
}