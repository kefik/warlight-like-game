namespace WinformsUI.InGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using GameAi;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameUser;
    using Phases;

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

        private Game game;

        private IEnumerable<Player> localPlayers;
        private IEnumerator<Player> playerOnTurn;
        private BeginGamePhaseControl beginGamePhaseControl;
        private BeginRoundPhaseControl beginRoundPhaseControl;
        private TurnPhaseControl turnPhaseControl;

        private readonly List<GameRound> rounds = new List<GameRound>();

        public void Initialize(Game game)
        {
            if (game == null)
            {
                throw new ArgumentException();
            }
            // initialize game
            this.game = game;
            localPlayers = from player in game.Players
                           where player.GetType() == typeof(HumanPlayer)
                           let humanPlayer = (HumanPlayer)player
                           where humanPlayer.User.UserType == UserType.LocalUser
                                 || humanPlayer.User.UserType == UserType.MyNetworkUser
                           select player;
            // initialize player whos on turn atm
            playerOnTurn = localPlayers.GetEnumerator();

            NextPlayer();
            // initialize game state
            if (game.RoundNumber == 0)
            {
                GameStateChange(GameState.GameBeginning);
            }
            else if ((from player in game.Players
                      where player.ControlledRegions.Count != 0
                      select player).Count() <= 1) // TODO: not working
            {
                GameStateChange(GameState.GameEnd);
            }
            else
            {
                GameStateChange(GameState.RoundBeginning);
            }

            using (UtilsDbContext db = new UtilsDbContext())
            {
                MapInfo mapInfo = (from item in db.Maps
                                   where item.Id == game.Map.Id
                                   select item).Single();
                // initialize map processor
                mapHandlerControl.Initialize(MapImageProcessor.Create(game.Map, mapInfo.ImageColoredRegionsPath,
                    mapInfo.ColorRegionsTemplatePath, mapInfo.ImagePath));
            }
            mapHandlerControl.Refresh(game);
            Refresh();
        }

        private bool NextPlayer()
        {
            bool returnValue = playerOnTurn.MoveNext();
            if (returnValue && game.GameType == GameType.MultiplayerHotseat)
            {
                MessageBox.Show($"Player {playerOnTurn.Current} is on turn!");
            }
            return returnValue;
        }

        private void StartOverGameBeginningPhase(GameBeginningRound gameBeginningRound)
        {
            mapHandlerControl.StartOver(gameBeginningRound);
            beginGamePhaseControl.BeginningRound.SelectedRegions.Clear();
        }

        private readonly List<GameBeginningRound> gameBeginningRounds = new List<GameBeginningRound>();

        private void CommitGameBeginningPhase(GameBeginningRound gameBeginningRound)
        {
            switch (game.GameType)
            {
                case GameType.SinglePlayer:
                {
                    gameBeginningRounds.Add(gameBeginningRound);
                    // TODO: play bots

                    var bot = GameBot.FromGame(game, game.Players.FirstOrDefault(x => x.GetType() == typeof(AiPlayer)), GameBotType.MonteCarloTreeSearchBot);
                    //bot.FindBestMove();

                    GameBeginningRound round = GameBeginningRound.Process(gameBeginningRounds);
                    using (UtilsDbContext db = new UtilsDbContext())
                    {
                        game.Play(round);
                        game.Save(db);
                    }
                    GameStateChange(GameState.RoundBeginning);

                    mapHandlerControl.Refresh(game);
                    mapHandlerControl.RefreshImage();

                    break;
                }
                case GameType.MultiplayerHotseat:
                {
                    gameBeginningRounds.Add(gameBeginningRound);
                    if (NextPlayer())
                    {
                        GameStateChange(GameState.GameBeginning);
                    }
                    else
                    {
                        GameBeginningRound round = GameBeginningRound.Process(gameBeginningRounds);
                        using (UtilsDbContext db = new UtilsDbContext())
                        {
                            game.Play(round);
                            game.Save(db);
                        }
                        GameStateChange(GameState.RoundBeginning);

                        mapHandlerControl.Refresh(game);
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

        private void PlayRound()
        {
            GameRound combinedRound = GameRound.Process(rounds);
            game.Play(combinedRound);


            mapHandlerControl.Refresh(game);
        }


        private GameState state;

        public void GameStateChange(GameState newGameState)
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
                    beginGamePhaseControl = new BeginGamePhaseControl
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
                    beginRoundPhaseControl = new BeginRoundPhaseControl
                    {
                        Parent = gameStateMenuPanel,
                        Dock = DockStyle.Fill
                    };
                    beginRoundPhaseControl.OnBegin += () => { GameStateChange(GameState.Deploying); };
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
                        turnPhaseControl = new TurnPhaseControl
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
                                        rounds.Add(new GameRound(game.Id, turnPhaseControl.DeployingStructure,
                                            turnPhaseControl.AttackingStructure));

                                        // TODO: play bots

                                        PlayRound();
                                        rounds.Clear();

                                        using (UtilsDbContext db = new UtilsDbContext())
                                        {
                                            game.Save(db);
                                        }

                                        turnPhaseControl?.Dispose();
                                        turnPhaseControl = null;
                                        GameStateChange(GameState.RoundBeginning);
                                        break;
                                    }
                                    case GameType.MultiplayerHotseat:
                                    {
                                        rounds.Add(new GameRound(game.RoundNumber, turnPhaseControl.DeployingStructure,
                                            turnPhaseControl.AttackingStructure));

                                        if (NextPlayer())
                                        {
                                            // play the next player instead
                                            turnPhaseControl?.Dispose();
                                            turnPhaseControl = null;

                                            GameStateChange(GameState.RoundBeginning);
                                        }
                                        else
                                        {
                                            // play the whole round

                                            // TODO: play bots
                                            GameRound round = GameRound.Process(rounds);

                                            PlayRound();
                                            rounds.Clear();

                                            using (UtilsDbContext db = new UtilsDbContext())
                                            {
                                                game.Save(db);
                                            }

                                            turnPhaseControl?.Dispose();
                                            turnPhaseControl = null;

                                            for (int i = game.Players.Count - 1; i >= 0; i--)
                                            {
                                                if (game.Players[i].ControlledRegions.Count == 0)
                                                {
                                                    MessageBox.Show($"Player {game.Players[i].Name} has lost.");
                                                    game.Players.RemoveAt(i);
                                                }
                                            }
                                            if (game.Players.Count == 1)
                                            {
                                                // TODO: game quit
                                                MessageBox.Show($"Player {game.Players[0].Name} has won the game!");
                                                GameStateChange(GameState.GameEnd);
                                                return;
                                            }

                                            playerOnTurn = localPlayers.GetEnumerator();
                                            NextPlayer();

                                            GameStateChange(GameState.RoundBeginning);
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
                                        mapHandlerControl.ResetRound(new GameRound(game.Id,
                                            turnPhaseControl.DeployingStructure,
                                            turnPhaseControl.AttackingStructure));
                                        turnPhaseControl.DeployingStructure.ArmiesDeployed.Clear();
                                        turnPhaseControl.AttackingStructure.Attacks.Clear();
                                    }
                                    else
                                    {
                                        mapHandlerControl.ResetDeployingPhase(turnPhaseControl
                                            .DeployingStructure);
                                        turnPhaseControl.DeployingStructure.ArmiesDeployed.Clear();
                                    }
                                    break;
                                case GameState.Attacking:
                                    mapHandlerControl.ResetAttackingPhase(turnPhaseControl.AttackingStructure,
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
                case GameState.GameEnd:
                    beginGamePhaseControl?.Dispose();
                    beginGamePhaseControl = null;
                    beginRoundPhaseControl?.Dispose();
                    beginRoundPhaseControl = null;
                    turnPhaseControl?.Dispose();
                    turnPhaseControl = null;
                    break;
            }
            state = newGameState;
        }

        private void SeizeAttempt(Region region)
        {
            if (region == null)
            {
                return;
            }
            // already selected 2 regions
            if ((from round in gameBeginningRounds
                 from selectedRegions in round.SelectedRegions
                 where selectedRegions.Item2 == region
                 select round).Any())
            {
                return;
            }
            if (beginGamePhaseControl.BeginningRound.SelectedRegions.Count >= 2)
            {
                MessageBox.Show("You have chosen enough regions.");
                return;
            }


            bool containsSameRegion = beginGamePhaseControl
                .BeginningRound.SelectedRegions.Any(tuple => tuple.Item2 == region);
            // if he chose the very same region already, ignore this choice
            if (containsSameRegion)
            {
                return;
            }

            // add, recolor, refresh
            beginGamePhaseControl.BeginningRound.SelectedRegions.Add(
                new Tuple<Player, Region>(playerOnTurn.Current, region));
            mapHandlerControl.SeizeRegion(region, playerOnTurn.Current);
        }

        private void DeployAttempt(Region region, int addedArmy)
        {
            if (region == null || region.Owner == null || region.Owner != playerOnTurn.Current)
            {
                return;
            }
            // represents the already existing deployment entry
            Tuple<Region, int> regionRepresentingTuple =
            (from item in turnPhaseControl.DeployingStructure.ArmiesDeployed
             where item.Item1 == region
             select item).FirstOrDefault();

            if (addedArmy > 0)
            {
                // if cant deploy anything
                if (turnPhaseControl.DeployingStructure.GetUnitsLeftToDeploy(playerOnTurn.Current) <= 0)
                {
                    return;
                }
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
                    if (regionRepresentingTuple.Item2 <= regionRepresentingTuple.Item1.Army)
                    {
                        return;
                    }

                    // add it with army - 1
                    regionRepresentingTuple = new Tuple<Region, int>(
                        regionRepresentingTuple.Item1, regionRepresentingTuple.Item2 - 1);


                    turnPhaseControl.DeployingStructure.ArmiesDeployed.Add(regionRepresentingTuple);
                }
                else
                {
                    return;
                }
            }

            mapHandlerControl.Deploy(regionRepresentingTuple.Item1, regionRepresentingTuple.Item2);
        }

        private void AttackAttempt(Region previouslySelectedRegion, Region region)
        {
            AttackManagerForm attackManager = new AttackManagerForm
            {
                ArmyLowerLimit = 0,
                ArmyUpperLimit
                    = turnPhaseControl.AttackingStructure
                        .GetUnitsLeftToAttack(previouslySelectedRegion,
                            turnPhaseControl.DeployingStructure)
            };
            DialogResult dialogResult = attackManager.ShowDialog();
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
