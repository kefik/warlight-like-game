namespace TheAiGames.CommunicationObjects.TranslationUnits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Communication.CommandHandling.Tokens;
    using Communication.Shared;

    public class Translator : ITranslator
    {
        private const string Settings = "settings";
        private const string SetupMap = "setup_map";
        private const string UpdateMap = "update_map";
        private const string OpponentMoves = "opponent_moves";
        private const string PickStartingRegion = "pick_starting_region";
        private const string Go = "go";

        protected IDictionary<string, int> NameIdsMappingDictionary { get; } = new Dictionary<string, int>();
        
        private readonly SettingsTranslationUnit settingsTranslationUnit;

        public Translator(SettingsTranslationUnit settingsTranslationUnit)
        {
            this.settingsTranslationUnit = settingsTranslationUnit;
        }

        public ICommandToken Translate(string input)
        {
            string[] tokens = input.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            switch (tokens[0])
            {
                case Settings:
                    return settingsTranslationUnit.Translate(tokens.Skip(1));
                case UpdateMap:
                    return TranslateUpdateMap(tokens.Skip(1));
            }
            
            throw new NotImplementedException();
        }

        public string Translate(ICommandToken commandToken)
        {
            switch (commandToken.CommandTokenType)
            {
                case CommandTokenType.PlaceArmiesResponse:
                    throw new NotImplementedException();
                case CommandTokenType.AttackReponse:
                    throw new NotImplementedException();
                default:
                    return null;
            }
        }

        private UpdateMapToken TranslateUpdateMap(IEnumerable<string> tokens)
        {
            throw new NotImplementedException();
        }
    }
}