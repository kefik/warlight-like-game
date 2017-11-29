namespace TheAiGames.CommunicationObjects.Tests.TranslationUnits
{
    using Communication.Shared;
    using CommunicationObjects.TranslationUnits;
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
            translator.Translate("settings your_bot player1");
            translator.Translate("settings opponent_bot player2");
        }
    }
}