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
        public ICollection<int> StartingPickRegionsIds { get; set; }
        public int? MaxRoundsCount { get; private set; }
        public int? StartingArmyCount { get; private set; }
        // TODO: option to set it dynamically
        public bool IsFogOfWar { get; private set; } = true;

        #endregion

        private Restrictions restrictions;

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
            // create bot

            int myPlayerId = playerDictionary.First(x => x.Value == OwnerPerspective.Mine).Key;

            var mapMin = mapController.GetMap();
            botHandler = new WarlightAiBotHandler(GameBotType.MonteCarloTreeSearchBot,
                mapMin, Difficulty.Hard,
                (byte)myPlayerId,
                playerDictionary.Where(x => x.Value == OwnerPerspective.Enemy).Select(x => (byte)x.Key).ToArray(),
                IsFogOfWar, restrictions);

            // find the best move
            botHandler.FindBestMoveAsync();
            // TODO: solve request
            // stop evaluation in specified timespan
            botHandler.StopEvaluation(token.Timeout.Value - new TimeSpan(0, 0, 0, 0, milliseconds: 30)).Wait();
            
            if (!(botHandler.GetCurrentBestMove() is BotGameTurn bestMove))
            {
                throw new ArgumentException($"Invalid type of {nameof(bestMove)}");
            }

            botHandler.UseFixedDeploy(bestMove.Deployments);

            // return converted token
            return new PlaceArmiesResponseToken(bestMove.PlayerId, bestMove.Deployments.Select(x => (x.RegionId, x.Army)).ToList());
        }

        private ICommandToken ExecuteCommand(AttackRequestToken token)
        {
            botHandler.FindBestMoveAsync();

            // stop evaluation in specified timespan
            botHandler.StopEvaluation(token.Timeout.Value - new TimeSpan(0, 0, 0, 0, milliseconds: 30)).Wait();

            if (!(botHandler.GetCurrentBestMove() is BotGameTurn bestMove))
            {
                throw new ArgumentException($"Invalid type of {nameof(bestMove)}");
            }

            // return obtained attacks
            return new AttackResponseToken(bestMove.PlayerId, bestMove.Attacks.Select(x => (x.AttackingRegionId, x.DefendingRegionId, x.AttackingArmy)).ToList());
        }

        private ICommandToken ExecuteCommand(PickStartingRegionsRequestToken token)
        {
            int myPlayerId = playerDictionary.First(x => x.Value == OwnerPerspective.Mine).Key;
            int enemyPlayerId = playerDictionary
                .First(x => x.Value == OwnerPerspective.Enemy).Key;

            // specify restrictions
            if (restrictions == null)
            {
                restrictions = new Restrictions()
                {
                    GameBeginningRestrictions = new List<GameBeginningRestriction>()
                    {
                        // initialize restrictions for me
                        new GameBeginningRestriction()
                        {
                            RegionsPlayerCanChooseCount = StartingPickRegionsCount.Value,
                            RestrictedRegions = StartingPickRegionsIds,
                            PlayerId = myPlayerId
                        },
                        // initialize for enemy (he can pick any regions except those that were chosen for me)
                        new GameBeginningRestriction()
                        {
                            RegionsPlayerCanChooseCount = StartingPickRegionsCount.Value,
                            RestrictedRegions = mapController.GetMap().RegionsMin.Select(x =>x.Id).Except(StartingPickRegionsIds).ToList(),
                            PlayerId = enemyPlayerId
                        }
                    }
                };
            }
            // create bot
            
            var mapMin = mapController.GetMap();
            botHandler = new WarlightAiBotHandler(GameBotType.MonteCarloTreeSearchBot,
                mapMin, Difficulty.Hard,
                (byte)myPlayerId,
                playerDictionary.Where(x => x.Value == OwnerPerspective.Enemy).Select(x => (byte)x.Key).ToArray(),
                IsFogOfWar, restrictions);

            // find the best move
            botHandler.FindBestMoveAsync();
            // stop evaluation in specified timespan
            botHandler.StopEvaluation(token.Timeout.Value - new TimeSpan(0, 0, 0, 0, milliseconds: 30)).Wait();
            
            if (!(botHandler.GetCurrentBestMove() is BotGameBeginningTurn turn))
            {
                throw new ArgumentException();
            }

            // get picked starting regions
            return new PickStartingRegionsResponseToken(turn.SeizedRegionsIds.Select(x => x).ToList());
        }

        private ICommandToken ExecuteCommand(TimePerMoveToken token)
        {
            TimePerMove = token.Time;
            return null;
        }

        private ICommandToken ExecuteCommand(StartingRegionsToken token)
        {
            mapController.Start();

            StartingPickRegionsIds = token.RegionIds;

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

            botHandler = new WarlightAiBotHandler(
                GameBotType.MonteCarloTreeSearchBot, mapMin,
                Difficulty.Hard, (byte) myPlayerId,
                new byte[]
                {
                    (byte) playerDictionary.First(x =>
                        x.Value == OwnerPerspective.Enemy).Key
                }, IsFogOfWar, restrictions);
            
            return null;
        }
    }
}