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
                        var gameBeginningRounds = new List<GameBeginningRound> { gameBeginningRound };

                        var round = GameBeginningRound.Process(gameBeginningRounds.ToArray());

                        game.Play(round);

                        game.Save(db);
                    }

                    State = GameState.RoundBeginning;

                    NewRound();

                    break;
                case GameType.MultiplayerHotseat:
                    break;
                case GameType.MultiplayerNetwork:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void NewRound()
        {
            processor.Refresh(Game);
            RefreshImage();
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
            // reset controls
            beginGamePhaseControl?.Dispose();
            beginRoundPhaseControl?.Dispose();

            switch (newGameState)
            {
                case GameState.GameBeginning:
                    beginGamePhaseControl = new BeginGamePhaseControl()
                    {
                        Parent = gameStateMenuPanel,
                        Dock = DockStyle.Fill
                    };
                    beginGamePhaseControl.OnStartOver += StartOverGameBeginningPhase;
                    beginGamePhaseControl.OnCommit += CommitGameBeginningPhase;
                    beginGamePhaseControl.Show();
                    break;
                case GameState.RoundBeginning:
                    beginRoundPhaseControl = new BeginRoundPhaseControl()
                    {
                        Parent = gameStateMenuPanel,
                        Dock = DockStyle.Fill
                    };
                    beginRoundPhaseControl.OnBegin += () =>
                    {
                        State = GameState.Deploying;
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
                            turnPhaseControl?.Dispose();
                            turnPhaseControl = new TurnPhaseControl()
                            {
                                Parent = gameStateMenuPanel,
                                Dock = DockStyle.Fill
                            };
                            turnPhaseControl.OnStateChanged += gameState =>
                            {
                                state = gameState;
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

        Region previouslySelectedRegion;
        private void ImageClick(object sender, MouseEventArgs e)
        {
            var region = processor.GetRegion(e.X, e.Y);

            switch (State)
            {
                case GameState.GameBeginning:
                    {
                        if (region == null) return;
                        // already selected 2 regions
                        if (beginGamePhaseControl.BeginningRound.SelectedRegions.Count >= 2)
                        {
                            MessageBox.Show("You have chosen enough regions.");
                            return;
                        }
                        bool containsSameRegion = (from tuple in beginGamePhaseControl.BeginningRound.SelectedRegions
                                                   where tuple.Item2 == region
                                                   select tuple.Item2).Any();
                        // if he chose the very same region already, ignore this choice
                        if (containsSameRegion) return;

                        // add, recolor, refresh
                        processor.Recolor(region, Color.FromKnownColor(playerOnTurn.Current.Color));
                        beginGamePhaseControl.BeginningRound.SelectedRegions.Add(
                            new Tuple<Player, Region>(playerOnTurn.Current, region));
                        gameMapPictureBox.Refresh();
                        break;
                    }
                case GameState.RoundBeginning:
                    break;
                case GameState.Deploying:
                    {
                        if (region == null) return;
                        // if player clicked on his own region
                        if (playerOnTurn.Current == region.Owner)
                        {
                            var regionRepresentingTuple = (from item in turnPhaseControl.DeployingStructure.ArmiesDeployed
                                                           where item.Item1 == region
                                                           select item).FirstOrDefault();
                            switch (e.Button)
                            {
                                case MouseButtons.Left:
                                    // deployed already in this region
                                    if (regionRepresentingTuple != null)
                                    {
                                        // cuz its immutable, remove the region
                                        turnPhaseControl.DeployingStructure.ArmiesDeployed.Remove(regionRepresentingTuple);
                                        // add it with army + 1
                                        regionRepresentingTuple = new Tuple<Region, int>(
                                            regionRepresentingTuple.Item1, regionRepresentingTuple.Item2 + 1);
                                        turnPhaseControl.DeployingStructure.ArmiesDeployed.Add(regionRepresentingTuple);
                                    }
                                    else // didnt deploy in this region
                                    {
                                        // create new structure for this
                                        regionRepresentingTuple = new Tuple<Region, int>(
                                            region, region.Army + 1);
                                        turnPhaseControl.DeployingStructure.ArmiesDeployed.Add(regionRepresentingTuple);
                                    }
                                    break;
                                case MouseButtons.Right:
                                    // deployed already in this region
                                    if (regionRepresentingTuple != null)
                                    {
                                        // cuz its immutable, remove the region
                                        turnPhaseControl.DeployingStructure.ArmiesDeployed.Remove(
                                            regionRepresentingTuple);

                                        // if you want to go under regions current state, dont
                                        if (regionRepresentingTuple.Item2 <= regionRepresentingTuple.Item1.Army) return;

                                        // add it with army - 1
                                        regionRepresentingTuple = new Tuple<Region, int>(
                                            regionRepresentingTuple.Item1, regionRepresentingTuple.Item2 - 1);


                                        turnPhaseControl.DeployingStructure.ArmiesDeployed.Add(regionRepresentingTuple);
                                    }
                                    else return; // didnt deploy => has nothing to remove
                                    break;
                            }

                            processor.OverDrawArmyNumber(regionRepresentingTuple.Item1, regionRepresentingTuple.Item2);
                            RefreshImage();
                        }
                        break;
                    }
                case GameState.Attacking:
                    {
                        if (region == null) return;
                        if (region.Owner != playerOnTurn.Current) return;
                        // TODO!!!!!
                        // this one is the first selected region
                        if (previouslySelectedRegion == null && region != null)
                        {
                            // highlight the region

                        }
                        else if (region == previouslySelectedRegion
                            && region == null)
                        {
                            // unhighlight both of them

                            region = null;
                            previouslySelectedRegion = null;
                        }
                        else if (previouslySelectedRegion.NeighbourRegions
                            .Any(x => x == region))
                        {
                            // do attack
                        }

                        previouslySelectedRegion = region;
                        break;
                    }
                case GameState.Committing:
                    break;
                case GameState.Committed:
                    break;
            }

        }

        private void ImageHover(object sender, MouseEventArgs e)
        {
            // TODO: fix flickering
            //if (processor.GetRegion(e.X, e.Y) != null) Cursor.Current = Cursors.Hand;
            //else Cursor.Current = Cursors.Default;

            menuButton.Text = string.Format($"X: {e.X}, Y: {e.Y}");

        }

        private void ImageSizeChanged(object sender, EventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;

            if (pictureBox == null) return;
        }


    }
}
