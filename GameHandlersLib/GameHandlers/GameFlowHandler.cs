namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using MapImageProcessor = MapHandlers.MapImageProcessor;

    /// <summary>
    ///     Component handling game state changes and all reactions to them.
    /// </summary>
    public abstract class GameFlowHandler
    {
        public Game Game { get; }

        public MapImageProcessor ImageProcessor { get; }

        /// <summary>
        /// Represents human player that is currently on turn.
        /// </summary>
        public virtual HumanPlayer PlayerOnTurn { get; protected set; }

        /// <summary>
        /// Represents last round.
        /// </summary>
        public IList<Tuple<Player, Round>> LastRound { get; protected set; } = new List<Tuple<Player, Round>>();

        /// <summary>
        /// Represents last turn.
        /// </summary>
        public Tuple<Player, Round> LastTurn
        {
            get
            {
                if (LastRound.Count - 1 < 0)
                {
                    return null;
                }
                return LastRound[LastRound.Count - 1];
            }
            protected set
            {
                LastRound.Add(value);
            }
        }

        protected IList<Round> AllRounds = new List<Round>();

        protected GameFlowHandler(Game game, MapImageProcessor processor)
        {
            Game = game;
            ImageProcessor = processor;
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
        /// Is invoked when round is played.
        /// </summary>
        public event Action OnRoundPlayed; 

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
            void PlayDeploying(GameRound round)
            {
                Deploying deploying = round.Deploying;
                foreach (var deployedArmies in deploying.ArmiesDeployed)
                {
                    Region region = (from item in Game.Map.Regions
                                     where item == deployedArmies.Region
                                     select item).First();
                    region.Army = deployedArmies.Army;
                }
            }

            void PlayAttacking(GameRound round)
            {
                var attacks = round.Attacking.Attacks;
                foreach (Attack attack in attacks)
                {
                    // TODO: real calculation according to rules
                    // get real attacker
                    Region attacker = (from region in Game.Map.Regions
                                       where region == attack.Attacker
                                       select region).First();

                    // if attacking region changed owner, cancel attack
                    if (attacker.Owner != attack.Attacker.Owner)
                    {
                        continue;
                    }

                    // get real defender
                    Region defender = (from region in Game.Map.Regions
                                       where region == attack.Defender
                                       select region).First();
                    // situation might have changed => recalculate attacking army
                    int realAttackingArmy = Math.Min(attack.AttackingArmy, attacker.Army);
                    // if they have same owner == just moving armies
                    if (defender.Owner == attacker.Owner)
                    {
                        // sum armies
                        defender.Army += realAttackingArmy;
                        // units were transfered
                        attacker.Army -= realAttackingArmy;
                    }
                    // attacking
                    else
                    {
                        Random random = new Random();

                        // calculate how many defending units were killed
                        int defendingArmyUnitsKilled = 0;
                        for (int i = 0; i < realAttackingArmy && defendingArmyUnitsKilled < defender.Army; i++)
                        {
                            double attackingUnitWillKillPercentage = random.Next(100) / 100d;

                            // attacking unit has 60% chance to kill defending unit
                            bool attackingUnitKills = attackingUnitWillKillPercentage < 0.6;
                            if (attackingUnitKills)
                            {
                                defendingArmyUnitsKilled++;
                            }
                        }

                        // calculate how many attacking army units were killed
                        int attackingArmyUnitsKilled = 0;
                        for (int i = 0; i < defender.Army && attackingArmyUnitsKilled < realAttackingArmy; i++)
                        {
                            double defendingUnitWillKillPercentage = random.Next(100) / 100d;

                            // defending unit has 70% chance to kill attacking unit
                            bool defendingUnitKills = defendingUnitWillKillPercentage < 0.7;
                            if (defendingUnitKills)
                            {
                                attackingArmyUnitsKilled++;
                            }
                        }
                        // attacker units were transfered
                        attacker.Army -= realAttackingArmy;

                        // army that survived as defender
                        int remainingDefendingArmy = defender.Army - defendingArmyUnitsKilled;
                        // army that remains after units being killed
                        int remainingAttackingArmy = realAttackingArmy - attackingArmyUnitsKilled;

                        // if region was conquered
                        if (remainingAttackingArmy > 0 && remainingDefendingArmy == 0)
                        {
                            // move rest of the units
                            remainingDefendingArmy = remainingAttackingArmy;
                            // region was conquered
                            defender.Owner = attack.Attacker.Owner;
                            // cuz of negative units
                            defender.Army = remainingDefendingArmy;
                        }
                        // none of the attackers survived
                        else if (remainingDefendingArmy == 0)
                        {
                            remainingDefendingArmy = 1;
                            defender.Army = remainingDefendingArmy;
                        }
                        // region was not conquered
                        else
                        {
                            // send rest of attacking units back
                            attacker.Army += remainingAttackingArmy;
                            // defenderArmy = what survived
                            defender.Army = remainingDefendingArmy;
                        }
                    }
                }
            }

            void PlayGameBeginningRound(GameBeginningRound round)
            {
                foreach (var roundSelectedRegion in round.SelectedRegions)
                {
                    Region realRegion = Game.Map.Regions.First(x => x == roundSelectedRegion.Region);
                    Player realPlayer = Game.Players.First(x => roundSelectedRegion.SeizingPlayer == x);

                    realRegion.Owner = realPlayer;
                }
            }

            Round linearizedRound = Round.Process(LastRound.Select(x => x.Item2).ToList());

            if (linearizedRound.GetType() == typeof(GameRound))
            {
                var convertedRound = (GameRound) linearizedRound;
                // deploying
                PlayDeploying(convertedRound);

                // attacking
                PlayAttacking(convertedRound);
            }
            else
            {
                var convertedRound = (GameBeginningRound) linearizedRound;
                PlayGameBeginningRound(convertedRound);
            }
            
            Game.Refresh();
            Game.RoundNumber++;

            AllRounds.Add(linearizedRound);
            LastRound.Clear();

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
            
            var lastTurn = (GameRound) LastTurn.Item2;

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
            var lastTurn = (GameRound) LastTurn.Item2;
            
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
                var lastTurn = (GameBeginningRound) LastTurn.Item2;

                var region = ImageProcessor.GetRegion(x, y);
                // seizes region
                lastTurn.SeizeRegion(PlayerOnTurn, region);

                ImageProcessor.Seize(region, PlayerOnTurn);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Print(e.Message);
                throw;
            }
            catch (ArgumentException e)
            {
                Debug.Print(e.Message);
#if DEBUG
                throw;
#endif
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
            else if (attacker != null && (ImageProcessor.GetRegion(x,y) == null || !ImageProcessor.GetRegion(x, y).IsNeighbourOf(attacker)))
            {
                // attacker correct, but next selected region is null or isnt neighbour
                return 1;
            }
            return ImageProcessor.Select(x, y, GetRealRegionArmy(ImageProcessor.GetRegion(x, y)));
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
            var lastTurn = (GameRound)LastTurn.Item2;
            return lastTurn.GetRegionArmy(region);
        }

        public int GetUnitsLeftToAttack(Region region)
        {
            if (region == null)
            {
                throw new ArgumentException("Region cannot be null.");
            }

            var lastTurn = (GameRound)LastTurn.Item2;
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
            var lastTurn = LastTurn.Item2;
            
            ImageProcessor.ResetRound(lastTurn);

            LastTurn.Item2.Reset();
        }

        /// <summary>
        /// Resets attacking phase.
        /// </summary>
        public void ResetAttacking()
        {
            var lastTurn = (GameRound) LastTurn.Item2;

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
            var lastTurn = (GameRound)LastTurn.Item2;

            lastTurn.Deploying.ResetDeploying();

            ImageProcessor.ResetDeployingPhase(lastTurn.Deploying);
        }

        /// <summary>
        /// Commits last turn. Checks if invariant match.
        /// </summary>
        public virtual void Commit()
        {
            if (LastTurn.Item2.GetType() == typeof(GameBeginningRound))
            {
                int seizedRegionsCount = ((GameBeginningRound) LastTurn.Item2).SelectedRegions.Count;
                if (seizedRegionsCount < 2)
                {
                    throw new ArgumentException($"Only {seizedRegionsCount} regions were selected.");
                }
            }
            else
            {
                var lastTurn = (GameRound) LastTurn.Item2;
            }
        }

        /// <summary>
        /// Starts the game or round. Initializes the GameFlowHandler to begin the round.
        /// </summary>
        public void Begin()
        {
            LastRound.Add(Game.RoundNumber == 0
                ? new Tuple<Player, Round>(PlayerOnTurn, new GameBeginningRound())
                : new Tuple<Player, Round>(PlayerOnTurn, new GameRound(Game.RoundNumber)));

            RedrawToPlayersPerspective();
        }
    }

    internal static class PlayerExtensions
    {
        /// <summary>
        /// Reports whether player is defeated.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsDefeated(this Player player, GameState state)
        {
            return player.ControlledRegions.Count == 0 && state != GameState.GameBeginning;
        }

        /// <summary>
        /// Reports how much army can player deploy based on what he has deployed already.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="deploying"></param>
        /// <returns></returns>
        public static int GetArmyLeftToDeploy(this Player player, Deploying deploying)
        {
            int deployedUnitsSum = (from deployment in deploying.ArmiesDeployed
                                      where deployment.Region.Owner == player
                                      select deployment.Army - deployment.Region.Army).Sum();
            return player.GetIncome() - deployedUnitsSum;
        }
    }
}
