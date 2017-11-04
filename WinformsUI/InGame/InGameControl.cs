namespace WinformsUI.InGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using Client.Entities;
    using GameAi;
    using GameHandlersLib.GameHandlers;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameUser;
    using Phases;
    using GameState = GameHandlersLib.GameHandlers.GameState;
    using MapImageProcessor = GameHandlersLib.MapHandlers.MapImageProcessor;

    public partial class InGameControl : UserControl
    {
        internal static GameState GameState { get; set; }

        private GameFlowHandler gameFlowHandler;

        private Game Game
        {
            get { return gameFlowHandler?.Game; }
        }

        private BeginGamePhaseControl beginGamePhaseControl;
        private BeginRoundPhaseControl beginRoundPhaseControl;
        private TurnPhaseControl turnPhaseControl;

        public InGameControl()
        {
            InitializeComponent();

            beginGamePhaseControl = new BeginGamePhaseControl
            {
                Parent = gameStateMenuPanel,
                Dock = DockStyle.Fill
            };
            beginGamePhaseControl.OnCommitted += () =>
            {
                bool isNext = gameFlowHandler.NextPlayer();
                if (isNext)
                {
                    beginGamePhaseControl.ResetControl();
                }
                else
                {
                    beginGamePhaseControl.Hide();
                    beginRoundPhaseControl.Show();
                    GameState = GameState.RoundBeginning;
                    gameFlowHandler.PlayRound();
                }
                gameFlowHandler.Begin();
            };

            beginRoundPhaseControl = new BeginRoundPhaseControl
            {
                Parent = gameStateMenuPanel,
                Dock = DockStyle.Fill
            };
            beginRoundPhaseControl.OnBegin += () =>
            {
                beginRoundPhaseControl.Hide();
                turnPhaseControl.Show();
                GameState = GameState.Deploying;
            };

            turnPhaseControl = new TurnPhaseControl
            {
                Parent = gameStateMenuPanel,
                Dock = DockStyle.Fill
            };
            turnPhaseControl.OnCommitted += () =>
            {
                bool isNext = gameFlowHandler.NextPlayer();
                turnPhaseControl.Hide();
                beginRoundPhaseControl.Show();
                GameState = GameState.RoundBeginning;
                if (!isNext)
                {
                    gameFlowHandler.PlayRound();
                }
                gameFlowHandler.Begin();
            };
        }

        public void Initialize(Game game)
        {
            if (game == null)
            {
                throw new ArgumentException("Game cannot be null.");
            }

            MapImageProcessor mapImageProcessor;
            using (UtilsDbContext db = new UtilsDbContext())
            {
                MapInfo mapInfo = (from item in db.Maps
                                   where item.Id == game.Map.Id
                                   select item).First();

                mapImageProcessor = MapImageProcessor.Create(game.Map, mapInfo.ImageColoredRegionsPath,
                    mapInfo.ColorRegionsTemplatePath, mapInfo.ImagePath, game.IsFogOfWar);
            }

            // initialize game
            switch (game.GameType)
            {
                case GameType.SinglePlayer:
                    gameFlowHandler = new SingleplayerGameFlowHandler(game, mapImageProcessor);
                    break;
                case GameType.MultiplayerHotseat:
                    gameFlowHandler = new HotseatGameFlowHandler(game, mapImageProcessor);
                    break;
                case GameType.MultiplayerNetwork:
                    throw new NotImplementedException();
            }

            // initialize map processor
            mapHandlerControl.Initialize(gameFlowHandler);
            gameFlowHandler.OnRoundPlayed += () =>
            {
                using (UtilsDbContext db = new UtilsDbContext())
                {
                    gameFlowHandler.Game.Save(db);
                }
            };
            gameFlowHandler.OnBegin += () =>
            {
                MessageBox.Show($"Player {gameFlowHandler.PlayerOnTurn.Name} is on turn.");
            };

            beginGamePhaseControl.Initialize(gameFlowHandler);
            turnPhaseControl.Initialize(gameFlowHandler);

            // initialize game state
            if (game.RoundNumber == 0)
            {
                GameState = GameState.GameBeginning;
                beginGamePhaseControl.Show();

                gameFlowHandler.Begin();
            }
            else if (game.IsFinished())
            {
                GameState = GameState.GameEnd;
            }
            else
            {
                GameState = GameState.RoundBeginning;

                beginGamePhaseControl.Hide();
                turnPhaseControl.Hide();
                beginRoundPhaseControl.Show();

                gameFlowHandler.Begin();
            }
        }
    }
}
