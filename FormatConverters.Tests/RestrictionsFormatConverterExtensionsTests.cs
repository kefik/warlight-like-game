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
    using GameObjectsLib.Tests;
    using NUnit.Framework;
    using Region = GameObjectsLib.GameMap.Region;

    [TestFixture]
    public class RestrictionsFormatConverterExtensionsTests
    {
        private GameObjectsRestrictions gameObjectsRestrictions;
        private Region czechia;
        private Region poland;
        private Region austria;
        private Region germany;
        
        [SetUp]
        public void Initialize()
        {
            var beginningRestrictions = new List<GameObjectsBeginningRestriction>();

            // set current directory to be the test bin directory (resharper using its own)
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var mapXmlString = TestMaps.TestMap;

            Map map;
            using (var stream = mapXmlString.ToStream())
            {
                map = new Map(1, "TEST", 1, stream);
            }
            czechia = map.Regions.First(x => x.Name == "Czechia");
            poland = map.Regions.First(x => x.Name == "Poland");
            austria = map.Regions.First(x => x.Name == "Austria");
            germany = map.Regions.First(x => x.Name == "Germany");

            var pc1 = new AiPlayer(Difficulty.Hard, "PC1", KnownColor.Beige, GameBotType.MonteCarloTreeSearchBot);
            var testUser = new HumanPlayer(new LocalUser("TestUser"), KnownColor.ActiveBorder);

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
        }
    }
}