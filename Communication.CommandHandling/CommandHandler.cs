namespace Communication.CommandHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameAi;
    using GameObjectsLib;
    using GameObjectsLib.GameRecording;
    using Shared;
    using Tokens;
    using Tokens.Settings;
    using Tokens.SetupMap;

    internal class CommandHandler : ICommandHandler
    {
        private readonly MapController mapController;
        private IBot<Turn> bot;

        private readonly IDictionary<int, OwnerPerspective> playerDictionary;

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

            throw new NotImplementedException();
        }

        private ICommandToken ExecuteCommand(AttackRequestToken token)
        {
            // TODO: solve request

            throw new NotImplementedException();
        }

        private ICommandToken ExecuteCommand(PickStartingRegionsRequestToken token)
        {
            // TODO: solve request

            throw new NotImplementedException();
        }

        private ICommandToken ExecuteCommand(TimePerMoveToken token)
        {
            TimePerMove = token.Time;
            return null;
        }

        private ICommandToken ExecuteCommand(StartingRegionsToken token)
        {
            mapController.Start();

            // create bot
            GameBotFactory factory = new GameBotFactory();

            int myPlayerId = playerDictionary.First(x => x.Value == OwnerPerspective.Mine).Key;

            bot = factory.Create(GameBotType.MonteCarloTreeSearchBot, mapController.GetMap(), Difficulty.Hard,
                (byte)myPlayerId, IsFogOfWar);

            // TODO: run to get the best move

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
            return null;
        }
    }
}