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
        public InGameControl()
        {
            InitializeComponent();

            mapHandlerControl.OnDeployAttempt += DeployAttempt;
            mapHandlerControl.OnAttackAttempt += AttackAttempt;
            mapHandlerControl.OnRegionSeizeAttempt += SeizeAttempt;
            mapHandlerControl.GetState = () => state;
            mapHandlerControl.GetPlayerOnTurn = () => playerOnTurn?.Current;
            // TODO: might throw exception
            
        }

        Game game;

        IEnumerable<Player> localPlayers;
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
                localPlayers = (from player in value.Players
                                    where player.GetType() == typeof(HumanPlayer)
                                    let humanPlayer = (HumanPlayer)player
                                    where humanPlayer.User.UserType == UserType.LocalUser
                                    || humanPlayer.User.UserType == UserType.MyNetworkUser
                                    select player);
                // initialize player whos on turn atm
                playerOnTurn = localPlayers.GetEnumerator();

                NextPlayer();
                // initialize game state
                State = game.RoundNumber == 0 ? GameState.GameBeginning : GameState.RoundBeginning;
                
                using (var db = new UtilsDbContext())
                {
                    var mapInfo = (from item in db.Maps
                                   where item.Id == game.Map.Id
                                   select item).Single();
                    // initialize map processor
                    mapHandlerControl.Processor = MapImageProcessor.Create(Game.Map, mapInfo.ImageColoredRegionsPath,
                        mapInfo.ColorRegionsTemplatePath, mapInfo.ImagePath);

                }
                mapHandlerControl.Processor.Refresh(value);
                Refresh();
            }
        }

        bool NextPlayer()
        {
            bool returnValue = playerOnTurn.MoveNext();
            if (returnValue && game.GameType == GameType.MultiplayerHotseat) MessageBox.Show($"Player {playerOnTurn.Current} is on turn!");
            return returnValue;
        }
        

        void StartOverGameBeginningPhase(GameBeginningRound gameBeginningRound)
        {
            mapHandlerControl.StartOver(gameBeginningRound);
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

                        mapHandlerControl.Processor.Refresh(Game);
                        mapHandlerControl.RefreshImage();

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
                            State = GameState.RoundBeginning;

                            mapHandlerControl.Processor.Refresh(Game);
                            mapHandlerControl.RefreshImage();

                            playerOnTurn = localPlayers.GetEnumerator();
                            NextPlayer();
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
            var combinedRound = Round.Process(rounds);
            game.Play(combinedRound);


            mapHandlerControl.Processor.Refresh(game);
            mapHandlerControl.RefreshImage();
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
        

        public void GameStateChanged(GameState newGameState)
        {
            // TODO: refactor
            // reset controls
            beginGamePhaseControl?.Dispose();
            beginGamePhaseControl = null;
            beginRoundPhaseControl?.Dispose();
            beginGamePhaseControl = null;

            switch (newGameState)
            {
                case GameState.GameBeginning:
                    turnPhaseControl?.Dispose();
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
                    turnPhaseControl?.Dispose();
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
                        mapHandlerControl.GetRegionArmy = turnPhaseControl.GetRealArmy;
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

                                            // TODO: play bots
                                            
                                            PlayRound();
                                            rounds.Clear();

                                            using (var db = new UtilsDbContext())
                                            {
                                                game.Save(db);
                                            }
                                            
                                            
                                            turnPhaseControl?.Dispose();
                                            turnPhaseControl = null;
                                            State = GameState.RoundBeginning;
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

                                                turnPhaseControl?.Dispose();
                                                turnPhaseControl = null;
                                            }
                                            else
                                            {
                                                // play the whole round

                                                // TODO: play bots
                                                var round = Round.Process(rounds);

                                                using (var db = new UtilsDbContext())
                                                {
                                                    game.Save(db);
                                                }
                                                
                                                PlayRound();

                                                turnPhaseControl?.Dispose();
                                                turnPhaseControl = null;
                                            }

                                            break;
                                        }
                                    case GameType.MultiplayerNetwork:
                                        // TODO
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
                                        mapHandlerControl.Processor.ResetRound(new Round(game.Id, turnPhaseControl.DeployingStructure,
                                            turnPhaseControl.AttackingStructure));
                                        turnPhaseControl.DeployingStructure.ArmiesDeployed.Clear();
                                        turnPhaseControl.AttackingStructure.Attacks.Clear();
                                    }
                                    else
                                    {
                                        mapHandlerControl.Processor.ResetDeployingPhase(turnPhaseControl.DeployingStructure);
                                        turnPhaseControl.DeployingStructure.ArmiesDeployed.Clear();
                                    }
                                    break;
                                case GameState.Attacking:
                                    mapHandlerControl.Processor.ResetAttackingPhase(turnPhaseControl.AttackingStructure,
                                        turnPhaseControl.DeployingStructure);
                                    turnPhaseControl.AttackingStructure.Attacks.Clear();
                                    break;
                                case GameState.Committing:
                                    break;
                            }
                            mapHandlerControl.RefreshImage();
                        };
                        turnPhaseControl.Show();
                    }
                    break;


            }
        }

        void SeizeAttempt(Region region)
        {
            if (region == null) return;
            // already selected 2 regions
            if (beginGamePhaseControl.BeginningRound.SelectedRegions.Count >= 2)
            {
                MessageBox.Show("You have chosen enough regions.");
                return;
            }
            bool containsSameRegion = beginGamePhaseControl
                .BeginningRound.SelectedRegions.Any(tuple => tuple.Item2 == region);
            // if he chose the very same region already, ignore this choice
            if (containsSameRegion) return;

            // add, recolor, refresh
            beginGamePhaseControl.BeginningRound.SelectedRegions.Add(
                new Tuple<Player, Region>(playerOnTurn.Current, region));
            mapHandlerControl.SeizeRegion(region, playerOnTurn.Current);
        }

        void DeployAttempt(Region region, int addedArmy)
        {
            if (region == null || region.Owner == null) return;
            // represents the already existing deployment entry
            var regionRepresentingTuple = (from item in turnPhaseControl.DeployingStructure.ArmiesDeployed
                                           where item.Item1 == region
                                           select item).FirstOrDefault();

            if (addedArmy > 0)
            {
                // if cant deploy anything
                if (turnPhaseControl.DeployingStructure.GetUnitsLeftToDeploy(playerOnTurn.Current) <= 0) return;
                // if the entry exists
                if (regionRepresentingTuple != null)
                {
                    // cuz its immutable, remove the region
                    turnPhaseControl.DeployingStructure.ArmiesDeployed.Remove(regionRepresentingTuple);
                    // add it with army + 1
                    regionRepresentingTuple = new Tuple<Region, int>(
                        regionRepresentingTuple.Item1, regionRepresentingTuple.Item2 + 1);
                    turnPhaseControl.DeployingStructure.ArmiesDeployed.Add(regionRepresentingTuple);
                }
                else
                {
                    // create new structure for this
                    regionRepresentingTuple = new Tuple<Region, int>(
                        region, region.Army + 1);
                    turnPhaseControl.DeployingStructure.ArmiesDeployed.Add(regionRepresentingTuple);
                }
            }
            else if (addedArmy < 0)
            {
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
                else return;
            }

            mapHandlerControl.Deploy(regionRepresentingTuple.Item1, regionRepresentingTuple.Item2);

        }

        void AttackAttempt(Region previouslySelectedRegion, Region region)
        {
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
        }
        

    }
}
