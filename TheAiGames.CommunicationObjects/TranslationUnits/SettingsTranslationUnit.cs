namespace TheAiGames.CommunicationObjects.TranslationUnits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Communication.CommandHandling.Tokens.Settings;
    using Communication.Shared;

    public class SettingsTranslationUnit
    {
        private const string TimeBank = "timebank";
        private const string YourBot = "your_bot";
        private const string OpponentBot = "opponent_bot";

        private readonly IDictionary<string, int> nameIdsMappingDictionary;

        public SettingsTranslationUnit(IDictionary<string, int> nameIdsMappingDictionary)
        {
            this.nameIdsMappingDictionary = nameIdsMappingDictionary;
        }

        public ICommandToken Translate(IEnumerable<string> tokens)
        {
            switch (tokens.First())
            {
                case TimeBank:
                    return CreateTimeBankToken(tokens.Skip(1));
                case YourBot:
                    nameIdsMappingDictionary.Add(new KeyValuePair<string, int>(tokens.First(), 1));
                    return null;
                case OpponentBot:
                    nameIdsMappingDictionary.Add(new KeyValuePair<string, int>(tokens.First(), 2));
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private TimeBankToken CreateTimeBankToken(IEnumerable<string> tokens)
        {
            int timeBankInterval = int.Parse(tokens.First());
            return new TimeBankToken(timeBankInterval);
        }
    }
}