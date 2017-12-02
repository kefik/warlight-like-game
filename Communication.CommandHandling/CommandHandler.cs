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

        #endregion

        public CommandHandler()
        {
            mapController = new MapController(true);
            playerDictionary = new Dictionary<int, OwnerPerspective>();
        }

        public ICommandToken Execute(ICommandToken commandToken)
        {
            // dynamically dispatch for the token
            return Execute((dynamic)commandToken);
        }

        private ICommandToken Execute(TimeBankToken timeBankToken)
        {
            TimeBank = timeBankToken.TimeBankInterval;
            return null;
        }

        private ICommandToken Execute(SetupBotToken token)
        {
            int playerId = token.PlayerId;
            OwnerPerspective ownerPerspective = token.OwnerPerspective;

            playerDictionary.Add(playerId, ownerPerspective);
            return null;
        }

        private ICommandToken Execute(StartingPickRegionsCountToken token)
        {
            StartingPickRegionsCount = token.RegionsToPickCount;
            return null;
        }

        private ICommandToken Execute(PlaceArmiesRequestToken token)
        {
            // TODO: solve request

            throw new NotImplementedException();
        }

        private ICommandToken Execute(AttackRequestToken token)
        {
            // TODO: solve request

            throw new NotImplementedException();
        }

        private ICommandToken Execute(PickStartingRegionsRequestToken token)
        {
            // TODO: solve request

            throw new NotImplementedException();
        }

        private ICommandToken Execute(TimePerMoveToken token)
        {
            TimePerMove = token.Time;
            return null;
        }

        private ICommandToken Execute(StartingRegionsToken token)
        {
            mapController.Start();

            // create bot
            GameBotFactory factory = new GameBotFactory();

            int myPlayerId = playerDictionary.First(x => x.Value == OwnerPerspective.Mine).Key;

            bot = factory.Create(GameBotType.MonteCarloTreeSearchBot, mapController.GetMap(), Difficulty.Hard,
                (byte)myPlayerId);

            // TODO: run to get the best move

            return null;
        }

        private ICommandToken Execute(MaxRoundsToken token)
        {
            MaxRoundsCount = token.MaxRoundsCount;
            return null;
        }

        private ICommandToken Execute(StartingArmiesToken token)
        {
            StartingArmyCount = token.StartingArmySize;
            return null;
        }

        private ICommandToken Execute(ISetupMapToken token)
        {
            mapController.SetupMap(token);
            return null;
        }

        private ICommandToken Execute(UpdateMapToken token)
        {
            mapController.UpdateMap(token);
            return null;
        }
    }
}