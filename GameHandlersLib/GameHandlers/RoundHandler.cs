﻿namespace GameHandlersLib.GameHandlers
{
    using System;
    using System.Linq;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using MapHandlers;

    internal class RoundHandler
    {
        private readonly Game game;
        private Random random;

        /// <summary>
        /// Initializes instance of <see cref="RoundHandler"/> with <see cref="game"/> parameter.
        /// </summary>
        /// <param name="game">Game that will be used in methods called.</param>
        public RoundHandler(Game game)
        {
            this.game = game;
            random = new Random();
        }

        private void PlayGameBeginningRound(GameBeginningRound round)
        {
            foreach (var roundSelectedRegion in round.SelectedRegions)
            {
                Region realRegion = game.Map.Regions.First(x => x == roundSelectedRegion.Region);
                Player realPlayer = game.Players.First(x => roundSelectedRegion.SeizingPlayer == x);

                realRegion.Owner = realPlayer;
            }
        }

        private void PlayAttacking(GameRound round)
        {
            var attacks = round.Attacking.Attacks;
            foreach (Attack attack in attacks)
            {
                // TODO: real calculation according to rules
                // get real attacker
                Region attacker = game.Map.Regions.First(region => region == attack.Attacker);

                // if attacking region changed owner, cancel attack
                if (attacker.Owner != attack.Attacker.Owner)
                {
                    continue;
                }

                // get real defender
                Region defender = game.Map.Regions.First(x => x == attack.Defender);
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
                    // calculate defending army unis killed
                    int defendingArmyUnitsKilled = GetUnitsKilled(realAttackingArmy, defender.Army, Global.AttackingUnitWillKillProbability);

                    // calculate how many attacking army units were killed
                    int attackingArmyUnitsKilled = GetUnitsKilled(defender.Army, realAttackingArmy, Global.DefendingUnitWillKillProbability);

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

        /// <summary>
        /// Calculates units killed by specified parameter.
        /// </summary>
        /// <remarks>
        ///     Parameters passed are not verified.
        /// </remarks>
        /// <param name="killingArmy"></param>
        /// <param name="armyToKill"></param>
        /// <param name="probabilityToKill">Probability to kill, must be between 0 and 1, otherwise method will not work.</param>
        /// <returns></returns>
        private int GetUnitsKilled(int killingArmy, int armyToKill, double probabilityToKill)
        {
            int unitsKilled = 0;
            for (int i = 0; i < killingArmy && unitsKilled < armyToKill; i++)
            {
                double attackingUnitWillKillPercentage = random.Next(100) / 100d;

                // attacking unit has {probabilityToKill} percent chance to kill defending unit
                bool attackingUnitKills = attackingUnitWillKillPercentage < probabilityToKill;
                if (attackingUnitKills)
                {
                    unitsKilled++;
                }
            }

            return unitsKilled;
        }

        private void PlayDeploying(GameRound round)
        {
            Deploying deploying = round.Deploying;
            foreach (var deployedArmies in deploying.ArmiesDeployed)
            {
                Region region = game.Map.Regions.First(x => x == deployedArmies.Region);
                region.Army = deployedArmies.Army;
            }
        }

        /// <summary>
        /// Plays round passed in parameter, changing regions,... in game instance specified in constructor.
        /// </summary>
        /// <param name="linearizedRound"></param>
        public void PlayRound(Round linearizedRound)
        {
            if (linearizedRound.GetType() == typeof(GameRound))
            {
                var gameRound = (GameRound) linearizedRound;

                PlayDeploying(gameRound);

                PlayAttacking(gameRound);
            }
            else
            {
                var gameBeginningRound = (GameBeginningRound) linearizedRound;

                PlayGameBeginningRound(gameBeginningRound);
            }
        }
    }
}