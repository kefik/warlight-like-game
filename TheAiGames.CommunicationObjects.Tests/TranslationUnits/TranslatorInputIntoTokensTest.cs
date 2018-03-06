namespace TheAiGames.CommunicationObjects.Tests.TranslationUnits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Communication.CommandHandling.Tokens;
    using Communication.CommandHandling.Tokens.Settings;
    using Communication.CommandHandling.Tokens.SetupMap;
    using Communication.Shared;
    using CommunicationObjects.TranslationUnits;
    using GameAi;
    using GameAi.Data;
    using NUnit.Framework;

    [TestFixture]
    public class TranslatorInputIntoTokensTest
    {
        private Translator translator;

        [SetUp]
        public void Setup()
        {
            translator = new Translator();
        }

        [Test]
        public void SettingsTimeBankTest()
        {
            var token = translator.Translate("settings timebank 10000");

            Assert.AreEqual(token.GetType(), typeof(TimeBankToken));

            var convertedToken = (TimeBankToken)token;

            Assert.AreEqual(convertedToken.TimeBankInterval, new TimeSpan(0, 0, 0, 0, milliseconds: 10000));
        }

        [Test]
        public void SettingsBotsTest()
        {
            var token = (SetupBotToken)translator.Translate("settings your_bot player1");

            Assert.AreEqual(token.OwnerPerspective, OwnerPerspective.Mine);
            Assert.AreEqual(token.PlayerId, 1);

            var token2 = (SetupBotToken)translator.Translate("settings opponent_bot player2");

            Assert.AreEqual(token2.OwnerPerspective, OwnerPerspective.Enemy);
            Assert.AreEqual(token2.PlayerId, 2);
        }

        [Test]
        public void SettingsMaxRoundsTest()
        {
            var token = (MaxRoundsToken) translator.Translate("settings max_rounds 50");

            Assert.AreEqual(token.MaxRoundsCount, 50);
        }

        [Test]
        public void SetupMapSuperRegionsTest()
        {
            var token = translator.Translate("setup_map super_regions 1 2 2 5");

            var convertedToken = (SetupSuperRegionsToken)token;

            Assert.True(convertedToken.InitialChanges.Contains((1, 2)));
            Assert.True(convertedToken.InitialChanges.Contains((2, 5)));
            Assert.AreEqual(convertedToken.InitialChanges.Count, 2);
        }

        [Test]
        public void SetupRegionsTest()
        {
            var token = translator.Translate("setup_map regions 1 1 2 1 3 2 4 2 5 2");

            var convertedToken = (SetupRegionsToken)token;

            Assert.True(convertedToken.SetupRegionsInstructions.Contains((1, 1)));
            Assert.True(convertedToken.SetupRegionsInstructions.Contains((2, 1)));
            Assert.True(convertedToken.SetupRegionsInstructions.Contains((3, 2)));
            Assert.True(convertedToken.SetupRegionsInstructions.Contains((4, 2)));
            Assert.True(convertedToken.SetupRegionsInstructions.Contains((5, 2)));
            Assert.AreEqual(convertedToken.SetupRegionsInstructions.Count, 5);
        }

        [Test]
        public void SetupNeighboursTest()
        {
            var token = translator.Translate("setup_map neighbors 1 2,3,4 2 3 4 5");

            var convertedToken = (SetupNeighboursToken)token;

            // check symmetricity

            // all tuples
            var pairsOfRelations = (from initialization in convertedToken.NeighboursInitialization
                                    from neighbour in initialization.NeighboursIds
                                    select new
                                    {
                                        RegionId = initialization.RegionId,
                                        NeighbourRegionId = neighbour
                                    }).ToList();
            // all tuples have their other symmetric pair
            bool symmetric = pairsOfRelations
                .All(pair => pairsOfRelations
                .Any(otherPair => otherPair.NeighbourRegionId == pair.RegionId
                && otherPair.RegionId == pair.NeighbourRegionId));
            Assert.AreEqual(symmetric, true);

            // check region 1
            var one = convertedToken.NeighboursInitialization.Single(x => x.RegionId == 1);
            Assert.True(one.NeighboursIds.Contains(2));
            Assert.True(one.NeighboursIds.Contains(3));
            Assert.True(one.NeighboursIds.Contains(4));
            Assert.AreEqual(one.NeighboursIds.Count, 3);

            // check region 2
            var two = convertedToken.NeighboursInitialization.Single(x => x.RegionId == 2);
            Assert.True(two.NeighboursIds.Contains(3));
            Assert.AreEqual(two.NeighboursIds.Count, 2);

            // check region 3
            var three = convertedToken.NeighboursInitialization.Single(x => x.RegionId == 4);
            Assert.True(three.NeighboursIds.Contains(5));
            Assert.AreEqual(three.NeighboursIds.Count, 2);
        }

        [Test]
        public void SetupWastelandsTest()
        {
            var token = (SetupWastelandsToken)translator.Translate("setup_map wastelands 3");

            Assert.AreEqual(token.Regions.Count, 1);
            Assert.True(token.Regions.Contains(3));

            var token2 = (SetupWastelandsToken)translator.Translate("setup_map wastelands");

            Assert.AreEqual(token2.Regions.Count, 0);
        }

        [Test]
        public void PickStartingRegionsTest()
        {
            var token = (PickStartingRegionsRequestToken)translator.Translate("pick_starting_region 10000 2 4");

            Assert.AreEqual(token.Timeout, new TimeSpan(0, 0, 0, 0, milliseconds: 10000));
            Assert.AreEqual(token.RegionIds.Count, 2);
            Assert.True(token.RegionIds.Contains(2));
            Assert.True(token.RegionIds.Contains(4));
        }

        [Test]
        public void UpdateMapTest()
        {
            translator.Translate("settings your_bot player1");
            translator.Translate("settings opponent_bot player2");

            var token = (UpdateMapToken)translator.Translate("update_map 1 player1 2 2 player1 4 3 neutral 10 4 player2 5");

            Assert.AreEqual(4, token.Changes.Count);
            Assert.True(token.Changes.Contains((1, 1, 2)));
            Assert.True(token.Changes.Contains((2, 1, 4)));
            Assert.True(token.Changes.Contains((3, 0, 10)));
            Assert.True(token.Changes.Contains((4, 2, 5)));
        }
    }
}