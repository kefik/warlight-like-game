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
        private readonly IRegionMinEvaluator regionMinEvaluator;
        private readonly DistanceMatrix distanceMatrix;

        public AggressiveBotActionsGenerator(IRegionMinEvaluator regionMinEvaluator,
            MapMin mapMin)
        {
            this.regionMinEvaluator = regionMinEvaluator;
            distanceMatrix = new DistanceMatrix(mapMin.RegionsMin);
        }

        /// <summary>
        /// Generates bot game turn based on current state of the game.
        /// </summary>
        /// <param name="currentGameState">Current state of the game.</param>
        /// <returns></returns>
        public IReadOnlyList<BotGameTurn> Generate(PlayerPerspective currentGameState)
        {
            var botGameTurns = new List<BotGameTurn>();

            // hint: regions that are near valuable not owned places should be deployed to
            // and later attacked
            var regionsToDeploy = (from region in currentGameState.GetMyRegions()
                                   from neighbour in region.NeighbourRegionsIds
                                       .Select(x => currentGameState.GetRegion(x))
                                   where neighbour.OwnerId != currentGameState.PlayerId
                                   orderby regionMinEvaluator.GetValue(currentGameState, neighbour) descending
                                   select region.Id).Distinct();

            foreach (int regionId in regionsToDeploy.Take(10))
            {
                var copiedState = currentGameState.ShallowCopy();

                var botGameTurn = new BotGameTurn(copiedState.PlayerId);

                ref var region = ref copiedState.GetRegion(regionId);

                botGameTurn.Deployments.Add((region.Id, region.Army + currentGameState.GetMyIncome(),
                        currentGameState.PlayerId));

                UpdateGameStateAfterDeploying(ref copiedState, botGameTurn.Deployments);

                // attack on most valuable neighbours
                foreach (RegionMin regionMin in copiedState.GetMyRegions())
                {
                    // get neighbours ordered by their value
                    var neighbours = regionMin.NeighbourRegionsIds
                        .Select(x => copiedState.GetRegion(x))
                        .Where(x => x.OwnerId != copiedState.PlayerId)
                        .OrderByDescending(x => regionMinEvaluator.GetValue(copiedState, x));

                    // attack on first neighbour that doesnt have large army
                    var neighboursToAttack = neighbours
                        .Where(x => (regionMin.Army - 1) * 9d / 10 >= x.Army)
                        .ToList();

                    foreach (RegionMin neighbour in neighboursToAttack)
                    {
                        // check the condition once more
                        // (with every attack regionMin.Army changes)
                        if ((regionMin.Army - 1) * 9d / 10 >= neighbour.Army)
                        {
                            int attackingArmy;
                            // if I can attack more than 1 neighbour => don't use
                            // needlessly whole army, attack possibly with part
                            if (neighboursToAttack.Count >= 2)
                            {
                                attackingArmy = Math.Min(2 * (neighbour.Army
                                                              + new PlayerPerspective(copiedState.MapMin,
                                                                  neighbour.OwnerId).GetMyIncome()),
                                    region.Army - 1);
                            }
                            else
                            {
                                attackingArmy = region.Army - 1;
                            }

                            botGameTurn.Attacks.Add((copiedState.PlayerId, regionMin
                                .Id, attackingArmy, neighbour.Id));
                            ref var refRegionMin = ref currentGameState.GetRegion(region.Id);
                            refRegionMin.Army = refRegionMin.Army - attackingArmy;
                        }
                    }
                }

                // hint: move army from inland territory to the edge of the area
                var inlandRegionsWithArmy = from regionMin in currentGameState.MapMin.RegionsMin
                                            where regionMin.Army > 1 && regionMin.OwnerId == currentGameState.PlayerId
                                            where regionMin.NeighbourRegionsIds
                                                .All(x => currentGameState.GetRegion(x).OwnerId == currentGameState.PlayerId)
                                            select regionMin;
                foreach (RegionMin regionMin in inlandRegionsWithArmy)
                {
                    var closestRegion = currentGameState.GetClosestRegion(regionMin,
                        x => x.OwnerId != currentGameState.PlayerId);

                    var path = distanceMatrix.GetPath(regionMin, closestRegion);
                    var regionIdToMoveTo = path.Skip(1).First();

                    botGameTurn.Attacks.Add((currentGameState.PlayerId,
                        regionMin.Id, regionMin.Army - 1, regionIdToMoveTo));
                }

                botGameTurns.Add(botGameTurn);
            }

            return botGameTurns;
        }
    }
}