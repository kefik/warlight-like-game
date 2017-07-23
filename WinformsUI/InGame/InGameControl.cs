using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DatabaseMapping;
using GameObjectsLib.Game;
using System.Linq;
using System.Reflection;
using GameObjectsLib;
using GameObjectsLib.GameMap;
using GameObjectsLib.GameUser;
using WinformsUI.InGame.Phases;
using Region = GameObjectsLib.GameMap.Region;

namespace WinformsUI.InGame
{
    public enum GameState : byte
    {
        GameBeginning,
        RoundBeginning,
        Deploying,
        Attacking,
        Committing,
        Committed
    }
    public partial class InGameControl : UserControl
    {
        Game game;
        MapImageProcessor processor;
        
        IEnumerator<Player> playerOnTurn;
        BeginGamePhaseControl beginGamePhaseControl;
        BeginRoundPhaseControl beginRoundPhaseControl;
        TurnPhaseControl turnPhaseControl;
        
        public Game Game
        {
            get { return game; }
            set
            {
                if (game != null) throw new ArgumentException();
                // initialize game
                game = value;
                var localPlayers = (from player in value.Players
                              where player.GetType() == typeof(HumanPlayer)
                              let humanPlayer = (HumanPlayer)player
                              where humanPlayer.User.UserType == UserType.LocalUser
                              || humanPlayer.User.UserType == UserType.MyNetworkUser
                              select player);
                // initialize player whos on turn atm
                playerOnTurn = localPlayers.GetEnumerator();
                playerOnTurn.MoveNext();
                // initialize game state
                switch (game.RoundNumber)
                {
                    case 0:
                        State = GameState.GameBeginning;
                        break;
                    case 1:
                        State = GameState.RoundBeginning;
                        break;
                }
                using (var db = new UtilsDbContext())
                {
                    var mapInfo = (from item in db.Maps
                                   where item.Id == game.Map.Id
                                   select item).Single();
                    // initialize map processor
                    processor = MapImageProcessor.Create(Game.Map, mapInfo.ImageColoredRegionsPath,
                        mapInfo.ColorRegionsTemplatePath, mapInfo.ImagePath);
                    
                }
                processor.Refresh(value);

                gameMapPictureBox.Image = processor.MapImage;
                gameMapPictureBox.BackgroundImage = processor.TemplateImage;

                Refresh();

            }
        }

        void StartOverGameBeginningPhase(GameBeginningRound gameBeginningRound)
        {
            processor.ResetRound(gameBeginningRound);

            RefreshImage();
            beginGamePhaseControl.BeginningRound.SelectedRegions.Clear();
        }

        void CommitGameBeginningPhase(GameBeginningRound gameBeginningRound)
        {
            switch (game.GameType)
            {
                case GameType.SinglePlayer:
                    using (var db = new UtilsDbContext())
                    {
                        // TODO: play bots
                        var gameBeginningRounds = new List<GameBeginningRound> {gameBeginningRound};

                        var round = GameBeginningRound.Process(gameBeginningRounds.ToArray());
                        
                        game.Play(round);

                        game.Save(db);
                    }
                    break;
                case GameType.MultiplayerHotseat:
                    break;
                case GameType.MultiplayerNetwork:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        GameState state;
        GameState State
        {
            get { return state; }
            set
            {
                GameStateChanged(value);
                state = value;
            }
        }
        

        void RefreshImage()
        {
            gameMapPictureBox.Refresh();
        }


        public void GameStateChanged(GameState newGameState)
        {
            switch (newGameState)
            {
                case GameState.GameBeginning:
                    beginGamePhaseControl = new BeginGamePhaseControl()
                    {
                        Parent = gameMenuPanel,
                        Dock = DockStyle.Fill
                    };
                    beginGamePhaseControl.OnStartOver += StartOverGameBeginningPhase;
                    beginGamePhaseControl.OnCommit += CommitGameBeginningPhase;
                    beginGamePhaseControl.Show();
                    break;
                case GameState.RoundBeginning:
                    beginRoundPhaseControl = new BeginRoundPhaseControl()
                    {
                        Parent = gameMenuPanel,
                        Dock = DockStyle.Fill
                    };
                    beginRoundPhaseControl.Show();
                    break;
                case GameState.Committed:
                    break;
                case GameState.Deploying:
                case GameState.Attacking:
                case GameState.Committing:
                    switch (state) // what was previous state
                    {
                        case GameState.Deploying:
                        case GameState.Attacking:
                        case GameState.Committing:
                            // previous state was represented by same screen => dont create new one
                            break;
                        default:
                            turnPhaseControl = new TurnPhaseControl()
                            {
                                Parent = gameMenuPanel,
                                Dock = DockStyle.Fill
                            };
                            turnPhaseControl.Show();
                            break;
                    }
                    break;
            }
        }

        public InGameControl()
        {
            InitializeComponent();

            gameMapPictureBox.BackgroundImageLayout = ImageLayout.None;
        }
        
        private void ImageClick(object sender, MouseEventArgs e)
        {
            switch (State)
            {
                case GameState.GameBeginning:
                    var region = processor.GetRegion(e.X, e.Y);

                    if (region == null) return;
                    if (beginGamePhaseControl.BeginningRound.SelectedRegions.Count >= 2)
                    {
                        MessageBox.Show("You have chosen enough regions.");
                        return;
                    }
                    processor.Recolor(region, Color.FromKnownColor(playerOnTurn.Current.Color));
                    beginGamePhaseControl.BeginningRound.SelectedRegions.Add(new Tuple<Player, Region>(playerOnTurn.Current, region));
                    gameMapPictureBox.Refresh();
                    break;
                case GameState.RoundBeginning:
                    break;
                case GameState.Deploying:
                    break;
                case GameState.Attacking:
                    break;
                case GameState.Committing:
                    break;
                case GameState.Committed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        private void ImageHover(object sender, MouseEventArgs e)
        {
            // TODO: fix flickering
            //if (processor.GetRegion(e.X, e.Y) != null) Cursor.Current = Cursors.Hand;
            //else Cursor.Current = Cursors.Default;

        }

        private void ImageSizeChanged(object sender, EventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;

            if (pictureBox == null) return;
        }
        

    }
}
