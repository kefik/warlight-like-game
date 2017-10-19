namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        protected MapImageProcessor MapImageProcessor { get; }

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
            MapImageProcessor = processor;
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
                foreach (Tuple<Player, Region> roundSelectedRegion in round.SelectedRegions)
                {
                    Region realRegion = (from region in Game.Map.Regions
                                         where region == roundSelectedRegion.Item2
                                         select region).First();
                    Player realPlayer = (from player in Game.Players
                                         where player == roundSelectedRegion.Item1
                                         select player).First();

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
        /// <param name="army"></param>
        public void Deploy(Region region, int army)
        {
            var lastTurn = (GameRound) LastTurn.Item2;
            lastTurn.Deploying.ArmiesDeployed.Add(new Deploy(region, army));

            //MapImageProcessor.Recolor(region, region.Owner.Color);
            //MapImageProcessor.DrawArmyNumber(region, army);
            //MapImageProcessor.Refresh(Game);
        }

        /// <summary>
        /// Attacks from <see cref="attackingRegion"/> to <see cref="defendingRegion"/> with <see cref="army"/> units.
        /// </summary>
        /// <param name="attackingRegion"></param>
        /// <param name="army"></param>
        /// <param name="defendingRegion"></param>
        public void Attack(Region attackingRegion, int army, Region defendingRegion)
        {
            if (attackingRegion.Owner != PlayerOnTurn)
            {
                throw new ArgumentException("Player is not attacking with his own region");
            }
            var lastTurn = (GameRound) LastTurn.Item2;

            {
                int unitsThatCanAttack = lastTurn.Attacking.GetUnitsLeftToAttack(attackingRegion, lastTurn.Deploying);

                if (army > unitsThatCanAttack || army < 0)
                {
                    throw new ArgumentException("Player cannot attack with this number of units.");
                }
            }

            Attack attack = new Attack(attackingRegion, army, defendingRegion);

            lastTurn.Attacking.Attacks.Add(attack);
        }

        /// <summary>
        /// Redraws map to perspective of player on the turn.
        /// </summary>
        public void RedrawToPlayersPerspective()
        {
            MapImageProcessor.Refresh(Game, PlayerOnTurn);
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
