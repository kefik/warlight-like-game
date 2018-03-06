namespace TheAiGames.EngineCommHandler.Tests.TranslationUnits
{
    using System.Collections.Generic;
    using Communication.CommandHandling.Tokens;
    using Communication.Shared;
    using EngineCommHandler.TranslationUnits;
    using NUnit.Framework;

    [TestFixture]
    public class TranslatorTokensIntoInputTest
    {
        private ITranslator translator;

        [SetUp]
        public void SetUp()
        {
            translator = new Translator();

            // for initializing dictionary mapping player to id
            translator.Translate("settings your_bot player1"); // will get id 1
            translator.Translate("settings opponent_bot player2"); // will get id 2
        }

        [Test]
        public void PlaceArmiesResponseTest()
        {
            int playerId = 1;
            var placements = new List<(int RegionId, int Army)>()
            {
                (1, 2),
                (2, 5)
            };

            PlaceArmiesResponseToken responseToken = new PlaceArmiesResponseToken(playerId, placements);

            string translatedResponse = translator.Translate(responseToken);

            Assert.AreEqual("player1 place_armies 1 2, player1 place_armies 2 5", translatedResponse);
        }

        [Test]
        public void AttackTransferResponseTest()
        {
            int playerId = 1;
            var attacks = new List<(int AttackingRegionId, int DefendingRegionId, int Army)>()
            {
                (1, 2, 3),
                (2, 3, 8)
            };

            var attackResponseToken = new AttackResponseToken(playerId, attacks);

            string translatedResponse = translator.Translate(attackResponseToken);

            Assert.AreEqual("player1 attack/transfer 1 2 3, player1 attack/transfer 2 3 8", translatedResponse);
        }

        [Test]
        public void NoMovesTest()
        {
            int playerId = 2;

            var attackResponseToken = new AttackResponseToken(playerId);
            string translatedAttackResponse = translator.Translate(attackResponseToken);

            Assert.AreEqual("No moves", translatedAttackResponse);

            var placeArmiesToken = new PlaceArmiesResponseToken(playerId, null);
            string translatedPlaceArmies = translator.Translate(placeArmiesToken);

            Assert.AreEqual("No moves", translatedPlaceArmies);
        }
    }
}