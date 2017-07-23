using System;
using System.Drawing;
using System.Windows.Forms;
using DatabaseMapping;
using GameObjectsLib.Game;
using System.Linq;
using System.Reflection;
using GameObjectsLib;
using GameObjectsLib.GameMap;
using WinformsUI.InGame.Phases;
using Region = GameObjectsLib.GameMap.Region;

namespace WinformsUI.InGame
{
    enum GameState : byte
    {
        GameBeginning,
        WatchTurn,
        RoundBeginning,
        Deploying,
        Attacking,
        Committing,
        Committed,
        GameEnding
    }
    public partial class InGameControl : UserControl
    {
        Game game;
        MapImageProcessor processor;

        Player playerOnTurn;

        BeginGamePhaseControl beginGamePhaseControl;
        BeginRoundPhaseControl beginRoundPhaseControl;
        TurnPhaseControl turnPhaseControl;

        readonly Color regionNotVisibleColor = Color.FromArgb(155, 150, 122);
        readonly Color regionVisibleUnoccupiedColor = Color.FromArgb(217, 190, 180);

        public Game Game
        {
            get { return game; }
            set
            {
                if (game != null) throw new ArgumentException();

                game = value;
                round = new Round(value.RoundNumber);
                playerOnTurn = (from player in value.Players
                           where player.GetType() == typeof(HumanPlayer)
                           select player).First();
                switch (game.RoundNumber)
                {
                    case 0:
                        State = GameState.GameBeginning;
                        break;
                    case 1:
                        State = GameState.RoundBeginning;
                        break;
                    default:
                        State = GameState.WatchTurn;
                        break;
                }
                using (var db = new UtilsDbContext())
                {
                    var mapInfo = (from item in db.Maps
                                   where item.Id == game.Map.Id
                                   select item).Single();

                    processor = MapImageProcessor.Create(Game.Map, mapInfo.ImageColoredRegionsPath,
                        mapInfo.ColorRegionsTemplatePath, mapInfo.ImagePath);
                    
                }
                processor.Refresh(value);

                gameMapPictureBox.Image = processor.MapImage;
                gameMapPictureBox.BackgroundImage = processor.TemplateImage;

                Refresh();

            }
        }

        GameState state;
        GameState State
        {
            get { return state; }
            set
            {
                switch (value)
                {
                    case GameState.GameBeginning:
                        beginGamePhaseControl = new BeginGamePhaseControl()
                        {
                            Parent = gameMenuPanel,
                            Dock = DockStyle.Fill
                        };
                        beginGamePhaseControl.OnStartOver += () =>
                        {
                            foreach (var tuple in beginGamePhaseControl.BeginningRound.SelectedRegions)
                            {
                                var region = tuple.Item2;
                                
                                processor.Recolor(region, regionNotVisibleColor);
                                RefreshImage();
                            }

                            beginGamePhaseControl.BeginningRound.SelectedRegions.Clear();
                        };
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
                    case GameState.Deploying:
                    case GameState.Attacking:
                    case GameState.Committing:
                    case GameState.Committed:
                        switch (state) // what was previous state
                        {
                            case GameState.Deploying:
                            case GameState.Attacking:
                            case GameState.Committing:
                            case GameState.Committed:
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
                    case GameState.GameEnding:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                state = value;
            }
        }

        void RefreshImage()
        {
            gameMapPictureBox.Refresh();
        }




        public InGameControl()
        {
            InitializeComponent();

            gameMapPictureBox.BackgroundImageLayout = ImageLayout.None;
        }


        Round round;
        
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
                    processor.Recolor(region, Color.FromKnownColor(playerOnTurn.Color));
                    beginGamePhaseControl.BeginningRound.SelectedRegions.Add(new Tuple<Player, Region>(playerOnTurn, region));
                    gameMapPictureBox.Refresh();
                    break;
                case GameState.WatchTurn:
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
                case GameState.GameEnding:
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

        void NextPhase()
        {

        }

    }
}
