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

        readonly List<Round> rounds = new List<Round>();

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
                State = game.RoundNumber == 0 ? GameState.GameBeginning : GameState.RoundBeginning;
                
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

        readonly List<GameBeginningRound> gameBeginningRounds = new List<GameBeginningRound>();
        void CommitGameBeginningPhase(GameBeginningRound gameBeginningRound)
        {
            switch (game.GameType)
            {
                case GameType.SinglePlayer:
                    {
                        gameBeginningRounds.Add(gameBeginningRound);
                        // TODO: play bots
                        var round = GameBeginningRound.Process(gameBeginningRounds);
                        using (var db = new UtilsDbContext())
                        {
                            game.Play(round);
                            game.Save(db);
                        }
                        State = GameState.RoundBeginning;

                        processor.Refresh(Game);
                        RefreshImage();

                        break;
                    }
                case GameType.MultiplayerHotseat:
                    {
                        gameBeginningRounds.Add(gameBeginningRound);
                        if (playerOnTurn.MoveNext())
                        {
                            State = GameState.GameBeginning;
                        }
                        else
                        {
                            var round = GameBeginningRound.Process(gameBeginningRounds);
                            using (var db = new UtilsDbContext())
                            {
                                game.Play(round);
                                game.Save(db);
                            }
                        }
                        break;
                    }
                case GameType.MultiplayerNetwork:
                {
                    // TODO: send game beginning round to the database
                    break;
                }
            }
        }

        void PlayRound()
        {
            State = GameState.RoundBeginning;

            var combinedRound = Round.Process(rounds);
            game.Play(combinedRound);

            processor.Refresh(game);
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
            // TODO: refactor
            // reset controls
            beginGamePhaseControl?.Dispose();
            beginRoundPhaseControl?.Dispose();

            switch (newGameState)
            {
                case GameState.GameBeginning:
                    turnPhaseControl = null;
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
                    turnPhaseControl = null;
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
                    if (state != GameState.Deploying && state != GameState.Attacking
                        && state != GameState.Committing)
                    {
                        turnPhaseControl?.Dispose();
                        turnPhaseControl = new TurnPhaseControl()
                        {
                            Parent = gameStateMenuPanel,
                            Dock = DockStyle.Fill
                        };
                        turnPhaseControl.OnStateChanged += gameState =>
                        {
                            // TODO: problem: this phase depends on type of game, mb implement more switches or do it via inheritance
                            if (gameState == GameState.Committed)
                            {
                                switch (game.GameType)
                                {
                                    case GameType.SinglePlayer:
                                        {
                                            rounds.Add(new Round(game.Id, turnPhaseControl.DeployingStructure,
                                                turnPhaseControl.AttackingStructure));


                                            // TODO: play AI players
                                            PlayRound();

                                            using (var db = new UtilsDbContext())
                                            {
                                                game.Save(db);
                                            }

                                            break;
                                        }
                                    case GameType.MultiplayerHotseat:
                                        {
                                            rounds.Add(new Round(game.Id, turnPhaseControl.DeployingStructure,
                                                turnPhaseControl.AttackingStructure));

                                            if (playerOnTurn.MoveNext())
                                            {
                                                // play the next player instead
                                                State = GameState.RoundBeginning;
                                            }
                                            else
                                            {
                                                // play the whole round
                                                var round = Round.Process(rounds);

                                                using (var db = new UtilsDbContext())
                                                {
                                                    game.Save(db);
                                                }

                                                game.Play(round);
                                                PlayRound();
                                            }

                                            break;
                                        }
                                    case GameType.MultiplayerNetwork:
                                        // TODO:
                                        break;
                                }
                            }
                            state = gameState;
                        };
                        turnPhaseControl.OnReset += gameState =>
                        {
                            switch (gameState)
                            {
                                case GameState.Deploying:
                                    if (state >= GameState.Attacking)
                                    {
                                        processor.ResetRound(new Round(game.Id, turnPhaseControl.DeployingStructure,
                                            turnPhaseControl.AttackingStructure));
                                        turnPhaseControl.DeployingStructure.ArmiesDeployed.Clear();
                                        turnPhaseControl.AttackingStructure.Attacks.Clear();
                                    }
                                    else
                                    {
                                        processor.ResetDeployingPhase(turnPhaseControl.DeployingStructure);
                                        turnPhaseControl.DeployingStructure.ArmiesDeployed.Clear();
                                    }
                                    break;
                                case GameState.Attacking:
                                    processor.ResetAttackingPhase(turnPhaseControl.AttackingStructure,
                                        turnPhaseControl.DeployingStructure);
                                    turnPhaseControl.AttackingStructure.Attacks.Clear();
                                    break;
                                case GameState.Committing:
                                    break;
                            }
                            RefreshImage();
                        };
                        turnPhaseControl.Show();
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

                                    // if player has deployed all of his possible units
                                    if (turnPhaseControl.DeployingStructure.GetUnitsLeftToDeploy(playerOnTurn.Current) <= 0)
                                    {
                                        return;
                                    }
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
                        // this one is the first selected region
                        if (previouslySelectedRegion == null && region != null)
                        {
                            if (region?.Owner != playerOnTurn?.Current) return;
                            // highlight the region

                            processor.HighlightRegion(region, turnPhaseControl.GetRealArmy(region));

                            RefreshImage();

                        }
                        // already highlighted someone and the second one is a neighbour
                        else if (previouslySelectedRegion != null && previouslySelectedRegion.IsNeighbourOf(region))
                        {
                            // unhighlight both of them
                            processor.HighlightRegion(region, turnPhaseControl.GetRealArmy(region));
                            RefreshImage();

                            AttackManagerForm attackManager = new AttackManagerForm()
                            {
                                ArmyLowerLimit = 0,
                                ArmyUpperLimit
                                = turnPhaseControl.AttackingStructure
                                .GetUnitsLeftToAttack(previouslySelectedRegion,
                                turnPhaseControl.DeployingStructure)
                            };
                            var dialogResult = attackManager.ShowDialog();
                            // execute the attack
                            if (dialogResult == DialogResult.OK)
                            {
                                Attack attack = new Attack(previouslySelectedRegion, attackManager.AttackingArmy,
                                    region);
                                turnPhaseControl.AttackingStructure.Attacks.Add(attack);
                            }


                            processor.UnhighlightRegion(previouslySelectedRegion, playerOnTurn.Current, turnPhaseControl.GetRealArmy(previouslySelectedRegion));
                            processor.UnhighlightRegion(region, playerOnTurn.Current, turnPhaseControl.GetRealArmy(region));
                            region = null;
                            previouslySelectedRegion = null;
                            RefreshImage();
                        }
                        else
                        {
                            if (region != null)
                            {
                                processor.UnhighlightRegion(region, playerOnTurn.Current,
                                    turnPhaseControl.GetRealArmy(region));
                                region = null;
                            }
                            if (previouslySelectedRegion != null)
                            {
                                processor.UnhighlightRegion(previouslySelectedRegion, playerOnTurn.Current, turnPhaseControl.GetRealArmy(previouslySelectedRegion));
                                previouslySelectedRegion = null;
                            }


                            RefreshImage();
                        }

                        previouslySelectedRegion = region;
                        break;
                    }
                case GameState.Committing:
                    {
                        break;
                    }
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
