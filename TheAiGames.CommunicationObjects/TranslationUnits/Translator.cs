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
        private const string PlaceArmies = "place_armies";
        private const string AttackOrTransfer = "attack/transfer";

        protected IDictionary<string, int> NameIdsMappingDictionary { get; } = new Dictionary<string, int>();

        private readonly SettingsTranslationUnit settingsTranslationUnit;

        public Translator(SettingsTranslationUnit settingsTranslationUnit)
        {
            this.settingsTranslationUnit = settingsTranslationUnit;
        }

        public ICommandToken Translate(string input)
        {
            string[] tokens = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            switch (tokens[0])
            {
                case Settings:
                    return settingsTranslationUnit.Translate(tokens.Skip(1));
                case UpdateMap:
                    return TranslateUpdateMap(tokens.Skip(1));
                case Go:
                    return TranslateGo(tokens.Skip(1));
                default:
                    throw new ArgumentOutOfRangeException(nameof(tokens));
            }
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
            int counter = 0;

            // create tokens with counters so we know indexes
            var tokensWithCounter = from token in tokens
                                    select new { Token = token, Counter = counter++ };

            // take groups by 3 elements
            var groupedTokens = from tokenWithCounter in tokensWithCounter
                                group tokenWithCounter.Token by tokenWithCounter.Counter / 3;

            var changes = new List<(int RegionId, int Owner, int Army)>();

            foreach (var group in groupedTokens)
            {
                int regionId = -1;
                int owner = -1;
                int army = -1;

                int i = 0;
                // initialize region, owner and army
                foreach (var token in group)
                {
                    switch (i)
                    {
                        case 0:
                            regionId = int.Parse(token);
                            break;
                        case 1:
                            owner = int.Parse(token);
                            break;
                        case 2:
                            army = int.Parse(token);
                            break;
                        default:
                            throw new ArgumentException();
                    }
                    i++;
                }

                if (i != 3)
                {
                    throw new ArgumentException();
                }

                changes.Add((regionId, owner, army));
            }

            return new UpdateMapToken(changes);
        }

        private ICommandToken TranslateGo(IEnumerable<string> tokens)
        {
            switch (tokens.First())
            {
                case PlaceArmies:
                    return new PlaceArmiesRequestToken(new TimeSpan(0, 0, 0, 0, milliseconds: int.Parse(tokens.First())));
                case AttackOrTransfer:
                    return new AttackRequestToken(new TimeSpan(0, 0, 0, 0, milliseconds: int.Parse(tokens.First())));
                default:
                    throw new ArgumentOutOfRangeException(nameof(tokens));
            }
        }
    }
}