namespace TheAiGames.EngineCommHandler.TranslationUnits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Collections;
    using Communication.CommandHandling.Tokens.Settings;
    using Communication.Shared;
    using GameAi.Data;

    /// <summary>
    /// Handles translation of settings commands at the beginning of the game.
    /// </summary>
    internal class SettingsTranslationUnit
    {
        private const string TimeBank = "timebank";
        private const string YourBot = "your_bot";
        private const string OpponentBot = "opponent_bot";
        private const string StartingArmies = "starting_armies";
        private const string MaxRounds = "max_rounds";
        private const string TimePerMove = "time_per_move";
        private const string StartingRegions = "starting_regions";
        private const string StartingPickAmount = "starting_pick_amount";

        private readonly BidirectionalDictionary<string, int> namesIdsMappingDictionary;

        private int lastPlayerNumber;

        public SettingsTranslationUnit(BidirectionalDictionary<string, int> namesIdsMappingDictionary)
        {
            this.namesIdsMappingDictionary = namesIdsMappingDictionary;

            // player with id 0 will be defined as neutral
            namesIdsMappingDictionary.Add("neutral", 0);
        }

        /// <summary>
        /// Translates from tokens specified in parameter into command token.
        /// </summary>
        /// <param name="tokens">Tokens specifying command and its parameter.</param>
        /// <returns></returns>
        public ICommandToken Translate(IEnumerable<string> tokens)
        {
            switch (tokens.First())
            {
                case TimeBank:
                    return CreateTimeBankToken(tokens.Skip(1));
                case YourBot:
                    return SetupYourBot(tokens.Skip(1));
                case OpponentBot:
                    return SetupEnemyBot(tokens.Skip(1));
                case StartingArmies:
                    return SetupStartingArmies(tokens.Skip(1));
                case MaxRounds:
                    return SetupMaxRounds(tokens.Skip(1));
                case TimePerMove:
                    return SetupTimePerMove(tokens.Skip(1));
                case StartingRegions:
                    return SetupStartingRegions(tokens.Skip(1));
                case StartingPickAmount:
                    return SetupStartingPickRegionsCount(tokens.Skip(1));
                default:
                    throw new ArgumentOutOfRangeException(nameof(tokens), $"Command {tokens.First()} not supported.");
            }
        }

        private TimeBankToken CreateTimeBankToken(IEnumerable<string> tokens)
        {
            int timeBankInterval = int.Parse(tokens.First()); // in ms
            return new TimeBankToken(new TimeSpan(0, 0, 0, 0, milliseconds: timeBankInterval));
        }

        private SetupBotToken SetupYourBot(IEnumerable<string> tokens)
        {
            string name = tokens.First();

            namesIdsMappingDictionary.Add(new KeyValuePair<string, int>(name, ++lastPlayerNumber));

            SetupBotToken setupBotToken = new SetupBotToken(lastPlayerNumber, OwnerPerspective.Mine);

            return setupBotToken;
        }

        private SetupBotToken SetupEnemyBot(IEnumerable<string> tokens)
        {
            string name = tokens.First();

            namesIdsMappingDictionary.Add(new KeyValuePair<string, int>(name, ++lastPlayerNumber));

            SetupBotToken setupBotToken = new SetupBotToken(lastPlayerNumber, OwnerPerspective.Enemy);

            return setupBotToken;
        }

        private StartingArmiesToken SetupStartingArmies(IEnumerable<string> tokens)
        {
            return new StartingArmiesToken(int.Parse(tokens.First()));
        }

        private MaxRoundsToken SetupMaxRounds(IEnumerable<string> tokens)
        {
            return new MaxRoundsToken(int.Parse(tokens.First()));
        }

        private TimePerMoveToken SetupTimePerMove(IEnumerable<string> tokens)
        {
            int timeInMs = int.Parse(tokens.First());
            return new TimePerMoveToken(new TimeSpan(0, 0, 0, 0, milliseconds: timeInMs));
        }

        private StartingRegionsToken SetupStartingRegions(IEnumerable<string> tokens)
        {
            return new StartingRegionsToken(tokens.Select(int.Parse).ToArray());
        }

        private StartingPickRegionsCountToken SetupStartingPickRegionsCount(IEnumerable<string> tokens)
        {
            return new StartingPickRegionsCountToken(int.Parse(tokens.First()));
        }
    }
}