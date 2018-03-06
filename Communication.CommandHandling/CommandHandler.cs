namespace Communication.CommandHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GameAi;
    using GameAi.Data;
    using GameAi.Data.GameRecording;
    using GameAi.Data.Restrictions;
    using GameAi.Interfaces;
    using GameObjectsLib;
    using GameObjectsLib.GameRecording;
    using Shared;
    using Tokens;
    using Tokens.Settings;
    using Tokens.SetupMap;

    internal class CommandHandler : ICommandHandler
    {
        private readonly MapController mapController;
        private IOnlineBotHandler<BotTurn> botHandler;

        #region Mapping dictionaries
        private readonly IDictionary<int, OwnerPerspective> playerDictionary;
        #endregion

        #region Properties

        public TimeSpan? TimeBank { get; private set; }
        public TimeSpan? TimePerMove { get; private set; }
        public int? StartingPickRegionsCount { get; private set; }
        public int? MaxRoundsCount { get; private set; }
        public int? StartingArmyCount { get; private set; }
        // TODO: option to set it dynamically
        public bool IsFogOfWar { get; private set; } = true;

        #endregion

        public CommandHandler()
        {
            mapController = new MapController();
            playerDictionary = new Dictionary<int, OwnerPerspective>();
        }

        public ICommandToken Execute(ICommandToken commandToken)
        {
            // dynamically dispatch for the token
            return ExecuteCommand((dynamic)commandToken);
        }

        private ICommandToken ExecuteCommand(TimeBankToken timeBankToken)
        {
            TimeBank = timeBankToken.TimeBankInterval;
            return null;
        }

        private ICommandToken ExecuteCommand(SetupBotToken token)
        {
            int playerId = token.PlayerId;
            OwnerPerspective ownerPerspective = token.OwnerPerspective;

            playerDictionary.Add(playerId, ownerPerspective);
            return null;
        }

        private ICommandToken ExecuteCommand(StartingPickRegionsCountToken token)
        {
            StartingPickRegionsCount = token.RegionsToPickCount;
            return null;
        }

        private ICommandToken ExecuteCommand(PlaceArmiesRequestToken token)
        {
            // TODO: solve request
            // stop evaluation in specified timespan
            botHandler.StopEvaluation(token.Timeout.Value - new TimeSpan(0, 0, 0, 0, milliseconds: 30)).Wait();

            var bestMove = botHandler.GetCurrentBestMove();

            // TODO: get place armies
            return new PlaceArmiesResponseToken(bestMove.PlayerId, null);
        }

        private ICommandToken ExecuteCommand(AttackRequestToken token)
        {
            // stop evaluation in specified timespan
            botHandler.StopEvaluation(token.Timeout.Value - new TimeSpan(0, 0, 0, 0, milliseconds: 30)).Wait();

            var bestMove = botHandler.GetCurrentBestMove();

            // TODO: get attacks
            return new AttackResponseToken(bestMove.PlayerId);
        }

        private ICommandToken ExecuteCommand(PickStartingRegionsRequestToken token)
        {
            // stop evaluation in specified timespan
            botHandler.StopEvaluation(token.Timeout.Value - new TimeSpan(0, 0, 0, 0, milliseconds: 30)).Wait();

            var bestMove = botHandler.GetCurrentBestMove();

            // get picked starting regions
            return new PickStartingRegionsResponseToken(null);
        }

        private ICommandToken ExecuteCommand(TimePerMoveToken token)
        {
            TimePerMove = token.Time;
            return null;
        }

        private ICommandToken ExecuteCommand(StartingRegionsToken token)
        {
            mapController.Start();

            // specify restrictions
            var restrictions = new Restrictions()
            {
                GameBeginningRestrictions = new List<IGameBeginningRestriction>()
                {
                    new GameBeginningRestriction()
                    {
                        RegionsPlayerCanChooseCount = StartingPickRegionsCount.Value,
                        RestrictedRegions = token.RegionIds
                    }
                }
            };
            // create bot

            int myPlayerId = playerDictionary.First(x => x.Value == OwnerPerspective.Mine).Key;

            var mapMin = mapController.GetMap();
            botHandler = new WarlightAiBotHandler(GameBotType.MonteCarloTreeSearchBot, mapMin, Difficulty.Hard,
                (byte)myPlayerId, IsFogOfWar, restrictions);
            
            // find the best move
            botHandler.FindBestMoveAsync();

            return null;
        }

        private ICommandToken ExecuteCommand(MaxRoundsToken token)
        {
            MaxRoundsCount = token.MaxRoundsCount;
            return null;
        }

        private ICommandToken ExecuteCommand(StartingArmiesToken token)
        {
            StartingArmyCount = token.StartingArmySize;
            return null;
        }

        private ICommandToken ExecuteCommand(ISetupMapToken token)
        {
            mapController.SetupMap(token);
            return null;
        }

        private ICommandToken ExecuteCommand(UpdateMapToken token)
        {
            mapController.UpdateMap(token);
            
            // update == create new bot handler with refreshed map
            int myPlayerId = playerDictionary.First(x => x.Value == OwnerPerspective.Mine).Key;

            var mapMin = mapController.GetMap();
            
            return null;
        }
    }
}