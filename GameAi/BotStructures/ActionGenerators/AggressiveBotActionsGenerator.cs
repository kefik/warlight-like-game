namespace GameAi.BotStructures.ActionGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.StructureEvaluators;

    /// <summary>
    /// Represents action generator for bot that always plays aggressively.
    /// </summary>
    internal class AggressiveBotActionsGenerator : GameActionsGenerator, IGameActionsGenerator
    {
        public AggressiveBotActionsGenerator(IRegionMinEvaluator regionMinEvaluator,
            MapMin mapMin) : base(new DistanceMatrix(mapMin.RegionsMin), regionMinEvaluator)
        {
        }

        /// <summary>
        /// Generates bot game turn based on current state of the game.
        /// </summary>
        /// <param name="currentGameState">Current state of the game.</param>
        /// <returns></returns>
        public IReadOnlyList<BotGameTurn> Generate(PlayerPerspective currentGameState)
        {
            IEnumerable<BotGameTurn> botGameTurns = new List<BotGameTurn>();

            botGameTurns = botGameTurns.Concat(GenerateNoWaitAggressive(currentGameState));

            return botGameTurns.ToList();
        }

        private IEnumerable<BotGameTurn> GenerateNoWaitAggressive(PlayerPerspective playerPerspective)
        {
            var botTurns = new List<BotGameTurn>();
            // hint: regions that are near valuable not owned places should be deployed to
            // and later attacked
            var regionsToDeploy = (from region in playerPerspective.GetMyRegions()
                                   from neighbour in region.NeighbourRegionsIds
                                       .Select(x => playerPerspective.GetRegion(x))
                                   where neighbour.OwnerId != playerPerspective.PlayerId
                                   orderby RegionMinEvaluator.GetValue(playerPerspective, neighbour) descending
                                   select region.Id).Distinct();

            foreach (int regionToDeployId in regionsToDeploy)
            {
                var copiedState = playerPerspective.ShallowCopy();

                var botGameTurn = new BotGameTurn(copiedState.PlayerId);

                ref var regionToDeploy = ref copiedState.GetRegion(regionToDeployId);

                botGameTurn.Deployments.Add((regionToDeploy.Id, regionToDeploy.Army + playerPerspective.GetMyIncome(),
                    playerPerspective.PlayerId));

                UpdateGameStateAfterDeploying(ref copiedState, botGameTurn.Deployments);

                // attack on most valuable neighbours
                foreach (var regionToAttackFromId in copiedState.GetMyRegions().Select(x => x.Id))
                {
                    ref var regionToAttackFrom = ref copiedState.GetRegion(regionToAttackFromId);
                    // get neighbours ordered by their value
                    var neighbours = regionToAttackFrom.NeighbourRegionsIds
                        .Select(x => copiedState.GetRegion(x))
                        .Where(x => x.OwnerId != copiedState.PlayerId)
                        .OrderByDescending(x => RegionMinEvaluator.GetValue(copiedState, x));

                    var copiedRegionMin = regionToAttackFrom;
                    // attack on first neighbour that doesnt have large army
                    var neighboursToAttack = neighbours
                        .Where(x => (copiedRegionMin.Army - 1) * 9d / 10 >= x.Army)
                        .ToList();

                    foreach (RegionMin neighbour in neighboursToAttack)
                    {
                        // check the condition once more
                        // (with every attack regionMin.Army changes)
                        if ((regionToAttackFrom.Army - 1) * 9d / 10 >= neighbour.Army)
                        {
                            int attackingArmy;
                            // if I can attack more than 1 neighbour => don't use
                            // needlessly whole army, attack possibly with part
                            if (neighboursToAttack.Count >= 2)
                            {
                                attackingArmy = Math.Min(2 * (neighbour.Army
                                                              + new PlayerPerspective(copiedState.MapMin,
                                                                  neighbour.OwnerId).GetMyIncome()),
                                    regionToAttackFrom.Army - 1);
                            }
                            else
                            {
                                attackingArmy = regionToAttackFrom.Army - 1;
                            }


                            botGameTurn.Attacks.Add((copiedState.PlayerId, regionToAttackFrom
                                .Id, attackingArmy, neighbour.Id));
                            regionToAttackFrom.Army -= attackingArmy;
                        }
                    }
                }

                AppendRedistributeInlandArmy(copiedState, botGameTurn.Attacks);

                botTurns.Add(botGameTurn);
            }

            return botTurns;
        }
    }
}