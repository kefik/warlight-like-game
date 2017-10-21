namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Collections.Generic;
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

        protected MapImageProcessor ImageProcessor { get; }

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

        protected GameState GameState;

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

                        defender.Army -= defendingArmyUnitsKilled;

                        realAttackingArmy -= attackingArmyUnitsKilled;
                        attacker.Army -= attackingArmyUnitsKilled;

                        if (realAttackingArmy > 0 && defender.Army == 0)
                        {
                            defender.Army -= realAttackingArmy;
                            // region was conquered
                            defender.Owner = attack.Attacker.Owner;
                            // cuz of negative units
                            defender.Army = -defender.Army;
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
        }

        /// <summary>
        /// Deploys units to the region.
        /// </summary>
        /// <param name="region"></param>
        /// <param name="army">New army in the region.</param>
        public void Deploy(Region region, int army)
        {
            if (region == null)
            {
                throw new ArgumentException($"Region cannot be null.");
            }
            if (region.Owner == null)
            {
                throw new ArgumentException($"Region {region.Name} owner cannot be null.");
            }
            if (army < 0)
            {
                throw new ArgumentException($"Invalid deployed army number.");
            }
            
            var lastTurn = (GameRound) LastTurn.Item2;

            // if there exists deployment entry => remove that entry and create new one
            lastTurn.Deploying.AddDeployment(region, army);
            
            ImageProcessor.Deploy(region, army);
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
            ImageProcessor.Attack(army);
        }

        /// <summary>
        /// Seizes region specified in parameter.
        /// </summary>
        public bool Seize(int x, int y)
        {
            try
            {
                var lastTurn = (GameBeginningRound) LastTurn.Item2;

                var region = ImageProcessor.GetRegion(x, y);
                // seizes region
                lastTurn.SeizeRegion(PlayerOnTurn, region);

                ImageProcessor.Seize(region, PlayerOnTurn);

                return true;
            }
            catch (ArgumentException)
            {
                return false;
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
            return ImageProcessor.Select(x, y, GetRealRegionArmy(ImageProcessor.GetRegion(x, y)));
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

        internal int GetUnitsLeftToAttack(Region region)
        {
            var lastTurn = (GameRound)LastTurn.Item2;
            return lastTurn.GetUnitsLeftToAttack(region);
        }
    }

    internal static class PlayerExtensions
    {
        public static bool IsDefeated(this Player player, GameState state)
        {
            return player.ControlledRegions.Count == 0 && state != GameState.GameBeginning;
        }
    }
}
