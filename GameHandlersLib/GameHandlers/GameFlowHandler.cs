namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using FormatConverters;
    using GameAi;
    using GameAi.Data;
    using GameAi.Data.Restrictions;
    using GameAi.Interfaces;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;
    using IMapImageProcessor = MapHandlers.IMapImageProcessor;

    /// <summary>
    ///     Component handling game state changes and all reactions to them.
    /// </summary>
    public abstract class GameFlowHandler
    {
        public Game Game { get; }

        public IMapImageProcessor ImageProcessor { get; }

        private readonly RoundHandler roundHandler;

        /// <summary>
        /// Represents human player that is currently on turn.
        /// </summary>
        public virtual HumanPlayer PlayerOnTurn { get; protected set; }

        /// <summary>
        /// Represents last round.
        /// </summary>
        public Round LastRound { get; protected set; }

        /// <summary>
        /// Represents last turn.
        /// </summary>
        public Turn LastTurn
        {
            get
            {
                if (LastRound.Turns.Count - 1 < 0)
                {
                    return null;
                }
                return LastRound.Turns[LastRound.Turns.Count - 1];
            }
            protected set
            {
                LastRound.Turns.Add(value);
            }
        }

        protected IList<ILinearizedRound> AllRounds
        {
            get { return Game.AllRounds; }
        }

        protected GameFlowHandler(Game game, IMapImageProcessor processor)
        {
            Game = game;
            ImageProcessor = processor;
            roundHandler = new RoundHandler(game);

            if (Game.RoundNumber == 0)
            {
                LastRound = new GameBeginningRound();
            }
            else
            {
                LastRound = new GameRound();
            }
        }

        public GameState GameState { get; protected set; }

        /// <summary>
        /// Is invoked when image is changed (redrawn,...)
        /// </summary>
        public event Action OnImageChanged
        {
            add { ImageProcessor.OnImageChanged += value; }
            remove
            {
                // ReSharper disable once DelegateSubtraction
                ImageProcessor.OnImageChanged -= value;
            }
        }

        /// <summary>
        /// Invoked when player is on turn.
        /// </summary>
        public event Action OnBegin; 

        /// <summary>
        /// Is invoked when round is played.
        /// </summary>
        public event Action OnRoundPlayed;

        /// <summary>
        /// Is invoked on game end.
        /// </summary>
        public event Action<Player> OnEnd;

        public virtual void GameStateChange(GameState newGameState)
        {
            GameState = newGameState;
        }
        
        /// <summary>
        ///     Plays given round, calculating everything, moving this instance of
        ///     the game into position after the round was played.
        /// </summary>
        public virtual void PlayRound()
        {
            roundHandler.PlayRound(LastRound);

            // if I played a round it means that next round will surelly wont be GameBeginningRound
            LastRound = new GameRound();

            OnRoundPlayed?.Invoke();
        }

        /// <summary>
        /// Deploys units to the region.
        /// </summary>
        /// <param name="region"></param>
        /// <param name="bonusArmy">Bonus army to the region.</param>
        public void Deploy(Region region, int bonusArmy)
        {
            if (region == null)
            {
                throw new ArgumentException($"Region cannot be null.");
            }
            if (region.Owner == null)
            {
                throw new ArgumentException($"Region {region.Name} owner cannot be null.");
            }
            if (region.Owner != PlayerOnTurn)
            {
                throw new ArgumentException("You cannot deploy to regions you do not own.");
            }
            
            var lastTurn = (GameTurn) LastTurn;

            int newArmy = lastTurn.GetRegionArmy(region) + bonusArmy;
            if (bonusArmy > 0 && bonusArmy > PlayerOnTurn.GetArmyLeftToDeploy(lastTurn.Deploying))
            {
                // bonus army > 0 and is more than i can deploy
                throw new ArgumentException($"Cannot deploy. Your limit {PlayerOnTurn.GetIncome()} for deployed units would be exceeded.");
            }
            else if (bonusArmy < 0 && region.Army > newArmy)
            {
                throw new ArgumentException("You cannot deploy less than region had at this round beginning.");
            }
            // if there exists deployment entry => remove that entry and create new one
            lastTurn.Deploying.AddDeployment(region, newArmy);
            
            ImageProcessor.Deploy(region, newArmy);
        }

        /// <summary>
        /// Deploys units to region specified by (x,y) coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="army"></param>
        public void Deploy(int x, int y, int army)
        {
            Deploy(ImageProcessor.GetRegion(x, y), army);
        }

        /// <summary>
        /// Attacks with <see cref="army"/> units.
        /// </summary>>
        /// <param name="army"></param>
        public void Attack(int army)
        {
            var lastTurn = (GameTurn) LastTurn;
            
            // is 2 regions selected
            if (ImageProcessor.SelectedRegions.Count != 2)
            {
                throw new ArgumentException("Attack cannot be performed. Not proper number of regions was selected.");
            }

            var attacker = ImageProcessor.SelectedRegions[0];
            var defender = ImageProcessor.SelectedRegions[1];
            // is attacker neighbour of defender
            if (!attacker.IsNeighbourOf(defender))
            {
                throw new ArgumentException($"Defender {defender.Name} is not a neighbour to attacker {attacker.Name}");
            }

            // add to list of attacks
            lastTurn.Attacking.AddAttack(attacker, defender, army);
            
            // draw attacks
            ImageProcessor.Attack(GetRealRegionArmy(attacker));
        }

        /// <summary>
        /// Seizes region specified in parameter.
        /// </summary>
        public void Seize(int x, int y)
        {
            try
            {
                var lastTurn = (GameBeginningTurn) LastTurn;

                var region = ImageProcessor.GetRegion(x, y);
                // seizes region
                lastTurn.SeizeRegion(PlayerOnTurn, region);

                ImageProcessor.Seize(region, PlayerOnTurn);
            }
            catch (ArgumentNullException e)
            {
                Debug.Print(e.Message);
#if DEBUG
                throw;
#endif
            }
            catch (ArgumentException e)
            {
                Debug.Print(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Selects region on (x,y) if possible, returns number of selected regions.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Select(int x, int y)
        {
            var attacker = ImageProcessor.SelectedRegions.FirstOrDefault();
            if (attacker == null && (ImageProcessor.GetRegion(x, y) == null ||
                ImageProcessor.GetRegion(x, y).Owner != PlayerOnTurn))
            {
                // didnt select anything and select attempt is not players region
                return 0;
            }
            else if (attacker != null
                && (ImageProcessor.GetRegion(x,y) == null
                || !ImageProcessor.GetRegion(x, y)
                    .IsNeighbourOf(attacker)))
            {
                // attacker correct, but next selected region is null or isnt neighbour
                return 1;
            }
            return ImageProcessor.Select(x, y, PlayerOnTurn, GetRealRegionArmy(ImageProcessor.GetRegion(x, y)));
        }

        /// <summary>
        /// Resets selected regions. Returns how many were resetted.
        /// </summary>
        /// <returns></returns>
        public int ResetSelection()
        {
            return ImageProcessor.ResetSelection();
        }

        /// <summary>
        /// Switches to the next player, redrawing contents.
        /// </summary>
        /// <returns></returns>
        public abstract bool NextPlayer();

        /// <summary>
        /// Redraws map to perspective of player on the turn.
        /// </summary>
        protected void RedrawToPlayersPerspective()
        {
            ImageProcessor.RedrawMap(Game, PlayerOnTurn);
        }

        internal int GetRealRegionArmy(Region region)
        {
            var lastTurn = (GameTurn)LastTurn;
            return lastTurn.GetRegionArmy(region);
        }

        public int GetUnitsLeftToAttack(Region region)
        {
            if (region == null)
            {
                throw new ArgumentException("Region cannot be null.");
            }

            var lastTurn = (GameTurn)LastTurn;
            return lastTurn.GetUnitsLeftToAttack(region);
        }

        public int GetUnitsLeftToAttack(int x, int y)
        {
            return GetUnitsLeftToAttack(ImageProcessor.GetRegion(x, y));
        }

        /// <summary>
        /// Returns currently attacking region units.
        /// </summary>
        /// <returns></returns>
        public int GetUnitsLeftToAttackInAttackingRegion()
        {
            return GetUnitsLeftToAttack(ImageProcessor.SelectedRegions.FirstOrDefault());
        }

        /// <summary>
        /// Resets last turn.
        /// </summary>
        public void ResetTurn()
        {
            var lastTurn = LastTurn;
            
            ImageProcessor.ResetRound(lastTurn);

            LastTurn.Reset();
        }

        /// <summary>
        /// Resets attacking phase.
        /// </summary>
        public void ResetAttacking()
        {
            var lastTurn = (GameTurn) LastTurn;

            // redraw
            ImageProcessor.ResetAttackingPhase(lastTurn.Attacking, lastTurn.Deploying);

            // reset
            lastTurn.Attacking.ResetAttacking();
        }

        /// <summary>
        /// Resets deploying phase.
        /// </summary>
        public void ResetDeploying()
        {
            var lastTurn = (GameTurn)LastTurn;

            lastTurn.Deploying.ResetDeploying();

            ImageProcessor.ResetDeployingPhase(lastTurn.Deploying);
        }

        /// <summary>
        /// Commits last turn. Checks if invariant match.
        /// </summary>
        public virtual void Commit()
        {
            if (LastTurn.GetType() == typeof(GameBeginningTurn))
            {
                int seizedRegionsCount = ((GameBeginningTurn) LastTurn).SelectedRegions.Count;
                if (seizedRegionsCount < 2)
                {
                    throw new ArgumentException($"Only {seizedRegionsCount} regions were selected.");
                }
            }
            else
            {
                var lastTurn = (GameTurn) LastTurn;
            }

            foreach (AiPlayer aiPlayer in Game.Players.OfType<AiPlayer>())
            {
                var players = Game.Players;
                var objectsRestrictions = Game.ObjectsRestrictions;
                var playerIdsMapper = players.CreateCompressedAiMapper();

                var restrictions = objectsRestrictions.ToRestrictions()
                    .ToRemappedRestrictions(playerIdsMapper);

                var mapMin = Game.Map.ToMapMin(playerIdsMapper);
                var botHandler = new WarlightAiBotHandler(aiPlayer.BotType, mapMin, Difficulty.Hard, (byte)playerIdsMapper.GetNewId(aiPlayer.Id),
                    players.Select(x => (byte)playerIdsMapper.GetNewId(x.Id)).ToArray(), true, restrictions);

                var botTurnTask = Task.Run(() => botHandler.FindBestMoveAsync());
                botHandler.StopEvaluation(new TimeSpan(0, 0, 0, 0, 2000));
                var turn = botTurnTask.Result.ToTurn(Game.Map, players, playerIdsMapper);

                LastTurn = turn;
            }
        }

        /// <summary>
        /// Starts the game or round. Initializes the GameFlowHandler to begin the round.
        /// </summary>
        public virtual void Begin()
        {
            if (Game.RoundNumber == 0)
            {
                LastTurn = new GameBeginningTurn(PlayerOnTurn);
            }
            else
            {
                LastTurn = new GameTurn(PlayerOnTurn);
            }

            RedrawToPlayersPerspective();

            OnBegin?.Invoke();
            
            // find the best move
            // create a new task bcuz otherwise ui thread gets blocked
            //var bestMoveTask = Task.Run(() => botHandler.FindBestMoveAsync());
            //var bestMove = botHandler.FindBestMoveAsync().Result;
            //botHandler.StopEvaluation(new TimeSpan(0, 0, 2));
        }

        /// <summary>
        /// Ends the game.
        /// </summary>
        public virtual void End()
        {
            if (!Game.IsFinished())
            {
                throw new ArgumentException("Function End() cannot be invoked if the game is not finished.");
            }

            var player = Game.Players.FirstOrDefault(x => x.ControlledRegions.Count != 0);
            
            RedrawToPlayersPerspective();

            OnEnd?.Invoke(player);
        }
    }
}
