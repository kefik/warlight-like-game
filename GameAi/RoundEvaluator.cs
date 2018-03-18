namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Interfaces;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.Evaluators;

    internal class RoundEvaluator : IRandomInjectable, IRoundEvaluator
    {
        private Random random = new Random();

        public MapMin Evaluate(MapMin mapMin, BotRound round)
        {
            var turns = round.BotTurns;

            switch (turns.First())
            {
                case BotGameTurn _:
                {
                    BotGameTurn[] gameTurns = turns.Cast<BotGameTurn>().ToArray();
                    var linearized = Linearize(gameTurns);
                    return PlayBotGameRound(mapMin, linearized);
                }
                case BotGameBeginningTurn _:
                {
                    BotGameBeginningTurn[] gameBeginningTurns = turns.Cast<BotGameBeginningTurn>().ToArray();
                    var linearized = Linearize(gameBeginningTurns);
                    return PlayBotGameBeginningRound(mapMin, linearized);
                }

                default:
                    throw new ArgumentException();
            }
        }

        public MapMin Evaluate(MapMin mapMin, LinearizedBotRound botRound)
        {
            switch (botRound)
            {
                case LinearizedBotBotGameRound round:
                    return PlayBotGameRound(mapMin, round);
                case LinearizedBotBotGameBeginningRound round:
                    return PlayBotGameBeginningRound(mapMin, round);
                default:
                    throw new ArgumentException();
            }
        }

        internal LinearizedBotBotGameRound Linearize(BotGameTurn[] gameTurns)
        {
            var deploying = new List<(int RegionId, int Army, int DeployingPlayerId)>();
            var attacking = new List<(int AttackingPlayerId, int AttackingRegionId,
                int AttackingArmy, int DefendingRegionId)>();

            int index = 0;
            while (true)
            {
                // true if anything in this round of cycle was added
                // false if nothing happened => everything was solved => can break out
                bool didSomething = false;
                foreach (var turn in gameTurns)
                {
                    if (turn.Deployments.Count > index)
                    {
                        var (regionId, army, deployingPlayerId) = turn.Deployments[index];
                        deploying.Add((regionId, army, deployingPlayerId));
                    }

                    if (turn.Attacks.Count > index)
                    {
                        var attack = turn.Attacks[index];
                        attacking.Add(attack);
                        didSomething = true;
                    }
                }

                if (!didSomething)
                {
                    break;
                }

                index++;
            }
            return new LinearizedBotBotGameRound()
            {
                Attacks = attacking,
                Deployments = deploying
            };
        }

        internal LinearizedBotBotGameBeginningRound Linearize(BotGameBeginningTurn[] beginningTurns)
        {
            var seizes = (from turn in beginningTurns
                           from seize in turn.SeizedRegionsIds
                           select ((byte)turn.PlayerId, seize)).ToList();

            return new LinearizedBotBotGameBeginningRound()
            {
                SeizedRegionsIds = seizes
            };
        }

        internal MapMin PlayBotGameBeginningRound(MapMin map, LinearizedBotBotGameBeginningRound round)
        {
            map = map.ShallowCopy();

            foreach (var (seizingPlayerId, regionId) in round.SeizedRegionsIds)
            {
                ref var region = ref map.GetRegion(regionId);
                region.OwnerId = seizingPlayerId;
            }

            return map;
        }

        internal MapMin PlayBotGameRound(MapMin map, LinearizedBotBotGameRound round)
        {
            PlayDeploying(ref map, round);
            PlayAttacking(ref map, round);
            return map;
        }

        private void PlayDeploying(ref MapMin map, LinearizedBotBotGameRound round)
        {
            var deploying = round.Deployments;
            foreach ((int regionId, int army, int deployingPlayerId) in deploying)
            {
                ref var region = ref map.GetRegion(regionId);
                region.Army = army;
            }
        }

        private void PlayAttacking(ref MapMin map, LinearizedBotBotGameRound round)
        {
            foreach ((int attackingPlayerId, int attackingRegionId,
                int attackingArmy, int defendingRegionId) in round.Attacks)
            {
                ref var attacker = ref map.GetRegion(attackingRegionId);
                ref var defender = ref map.GetRegion(defendingRegionId);

                // attacking player changed => attack did not happen
                if (attacker.OwnerId != attackingPlayerId)
                {
                    continue;
                }

                int realAttackingArmy = Math.Min(attacker.Army - 1, attackingArmy);

                // attack did not happen, because there was no army to attack
                if (realAttackingArmy <= 0)
                {
                    continue;
                }

                // if they have same owner == just moving armies
                if (defender.OwnerId == attacker.OwnerId)
                {
                    // sum armies
                    defender.Army += realAttackingArmy;
                    // units were transfered
                    attacker.Army -= realAttackingArmy;
                }
                else
                {
                    // calculate defending army unis killed
                    int defendingArmyUnitsKilled = GetUnitsKilled(realAttackingArmy, defender.Army, 0.6);

                    // calculate how many attacking army units were killed
                    int attackingArmyUnitsKilled = GetUnitsKilled(defender.Army, realAttackingArmy, 0.7);

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
                        defender.OwnerId = (byte)attackingPlayerId;
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

        private int GetUnitsKilled(int killingArmy, int armyToKill, double probabilityToKill)
        {
            int unitsKilled = 0;
            for (int i = 0; i < killingArmy && unitsKilled < armyToKill; i++)
            {
                double attackingUnitWillKillPercentage = random.NextDouble();

                // attacking unit has {probabilityToKill} percent chance to kill defending unit
                bool attackingUnitKills = attackingUnitWillKillPercentage < probabilityToKill;
                if (attackingUnitKills)
                {
                    unitsKilled++;
                }
            }

            return unitsKilled;
        }

        void IRandomInjectable.Inject(Random random)
        {
            this.random = random;
        }
    }
}