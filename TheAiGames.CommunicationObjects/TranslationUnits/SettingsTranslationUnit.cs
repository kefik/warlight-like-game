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
        public ICommandToken Translate(IEnumerable<string> tokens)
        {
            switch (tokens.First())
            {
                case TimeBank:
                    return CreateTimeBankToken(tokens.Skip(1));
            }

            throw new NotImplementedException();
        }

        private TimeBankToken CreateTimeBankToken(IEnumerable<string> tokens)
        {
            int timeBankInterval = int.Parse(tokens.First());
            return new TimeBankToken(timeBankInterval);
        }
    }
}