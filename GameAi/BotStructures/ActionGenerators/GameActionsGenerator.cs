namespace GameAi.BotStructures.ActionGenerators
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.Evaluators.StructureEvaluators;

    internal abstract class GameActionsGenerator
    {
        protected DistanceMatrix DistanceMatrix;
        protected internal readonly IRegionMinEvaluator RegionMinEvaluator;

        protected GameActionsGenerator(DistanceMatrix distanceMatrix, IRegionMinEvaluator regionMinEvaluator)
        {
            this.DistanceMatrix = distanceMatrix;
            this.RegionMinEvaluator = regionMinEvaluator;
        }

        protected void UpdateGameStateAfterDeploying(ref PlayerPerspective gameState, ICollection<BotDeployment> deployments)
        {
            foreach (var deployment in deployments)
            {
                ref var region = ref gameState.GetRegion(deployment.RegionId);
                region.Army = deployment.Army;
            }
        }

        protected void UpdateGameStateAfterAttack(ref PlayerPerspective gameState, ICollection<BotAttack> attacks)
        {
            foreach (BotAttack attack in attacks)
            {
                ref var attackingRegion = ref gameState.GetRegion(attack.AttackingArmy);
                attackingRegion.Army -= attack.AttackingArmy;
            }
        }

        protected void AppendRedistributeInlandArmy(PlayerPerspective playerPerspective,
            ICollection<BotAttack> attacks)
        {
            // hint: move army from inland territory to the edge of the area
            // get those regions
            var inlandRegionsWithArmy = from regionMin in playerPerspective.MapMin.RegionsMin
                                        where regionMin.Army > 1 && regionMin.OwnerId == playerPerspective.PlayerId
                                        where regionMin.NeighbourRegionsIds
                                            .All(x => playerPerspective.GetRegion(x).OwnerId == playerPerspective.PlayerId)
                                        select regionMin;
            // for each inland region with army
            foreach (RegionMin regionMin in inlandRegionsWithArmy)
            {
                // get closest region that is not mine
                var closestRegion = playerPerspective.GetClosestRegion(regionMin,
                    x => x.OwnerId != playerPerspective.PlayerId);

                // get path
                var path = DistanceMatrix.GetPath(regionMin, closestRegion);
                // get first region of that path
                var regionIdToMoveTo = path.Skip(1).First();

                int armyToBeMoved = regionMin.Army - 1;
                // add to attacks
                attacks.Add(new BotAttack(playerPerspective.PlayerId,
                    regionMin.Id, armyToBeMoved, regionIdToMoveTo));

                // move the units
                ref var regionMinRef = ref playerPerspective.GetRegion(regionMin.Id);
                ref var regionToMoveTo = ref playerPerspective.GetRegion(regionIdToMoveTo);

                regionToMoveTo.Army += armyToBeMoved;
                regionMinRef.Army -= armyToBeMoved;
            }
        }

        /// <summary>
        /// Deploy near valuable Regions.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        /// <remarks>
        /// Each instance of return value represents one way to deploy. Deploying is done only by full available units.
        /// </remarks>
        protected IList<BotDeployment> DeployOffensively(PlayerPerspective currentGameState)
        {
            var botDeployment = new List<BotDeployment>();
            var playerPerspective = currentGameState;

            // neighbours of regions that are valuable and should be attacked
            var regionsToDeploy = (from region in playerPerspective.GetMyRegions()
                                   from neighbour in region.NeighbourRegionsIds
                                       .Select(x => playerPerspective.GetRegion(x))
                                   where neighbour.OwnerId != playerPerspective.PlayerId
                                   orderby RegionMinEvaluator.GetValue(playerPerspective, neighbour) descending
                                   select region).Distinct().ToList();

            int unitsToDeploy = currentGameState.GetMyIncome();
            foreach (RegionMin regionMin in regionsToDeploy.Take(5))
            {
                botDeployment.Add(
                    new BotDeployment(regionMin.Id, regionMin.Army + unitsToDeploy, currentGameState.PlayerId));
            }

            return botDeployment;
        }
        
        /// <summary>
        /// Deploy in regions near enemy strong force.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        protected IList<BotDeployment> DeployToCounterSecurityThreat(PlayerPerspective currentGameState)
        {
            var botDeployments = new List<BotDeployment>();

            var playerPerspective = currentGameState;
            var myRegions = currentGameState.GetMyRegions();

            // get my regions that have enemy army near them
            var regionsBySecurityThreat = myRegions
                // order by army enemy neighbours have in sum
                .OrderByDescending(x => x.NeighbourRegionsIds
                .Select(neighbourId => playerPerspective.GetRegion(neighbourId))
                .Where(neighbour => neighbour.OwnerId != 0 && neighbour.OwnerId != currentGameState.PlayerId)
                .Sum(neighbour => neighbour.Army));

            // deploy
            foreach (var region in regionsBySecurityThreat.Take(5))
            {
                botDeployments.Add(
                        new BotDeployment(region.Id, region.Army + playerPerspective.GetMyIncome(), currentGameState.PlayerId));
            }

            return botDeployments;
        }

        /// <summary>
        /// Generate all kinds of deployments.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        /// <remarks>Using this can be resource-exhausting due to many deployment options.</remarks>
        protected IEnumerable<BotDeployment> DeployToAll(PlayerPerspective currentGameState)
        {
            foreach (RegionMin regionMin in currentGameState.GetMyRegions())
            {
                yield return new BotDeployment(regionMin.Id, regionMin.Army + currentGameState.GetMyIncome(), currentGameState.PlayerId);
            }
        }

        /// <summary>
        /// Attacks only when there's no enemy around and the bot is sure it will win.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        protected void AttackSafely(PlayerPerspective currentGameState, IList<BotAttack> botAttacks)
        {
            foreach (var regionMin in currentGameState.GetMyRegions())
            {
                ref var refRegionMin = ref currentGameState.GetRegion(regionMin.Id);
                var neighboursToAttack = refRegionMin.NeighbourRegionsIds
                    .Select(x => currentGameState.GetRegion(x))
                    .Where(x => x.OwnerId != currentGameState.PlayerId)
                    .ToList();

                // if theres any neighbour where is the enemy, don't attack (we want to attack safely only)
                if (neighboursToAttack.Any(x => x.OwnerId != 0 && x.OwnerId != currentGameState.PlayerId))
                {
                    continue;
                }

                foreach (RegionMin neighbourToAttack in neighboursToAttack)
                {
                    var neighboursNeighbours = neighbourToAttack.NeighbourRegionsIds
                        .Select(x => currentGameState.GetRegion(x))
                        .OrderByDescending(x => RegionMinEvaluator.GetValue(currentGameState, x));

                    // I have large enough army and
                    // theres no neighbour of neighbour that is not mine => attack
                    if (refRegionMin.Army - 1 > neighbourToAttack.Army * 10d / 7
                        && !neighboursNeighbours.Any(x => x.OwnerId != 0 && x.OwnerId != currentGameState.PlayerId))
                    {
                        // don't attack with too large force
                        int attackingArmy = Math.Min(neighbourToAttack.Army * 3, refRegionMin.Army - 1);

                        botAttacks.Add(new BotAttack(currentGameState.PlayerId, refRegionMin.Id,
                            attackingArmy, neighbourToAttack.Id));

                        refRegionMin.Army -= attackingArmy;
                    }
                }
            }
        }

        protected void AttackAggressively(PlayerPerspective currentGameState, IList<BotAttack> botAttacks)
        {
            var myRegions = currentGameState.GetMyRegions().Select(x => x.Id);

            foreach (var myRegionId in myRegions)
            {
                ref var myRegion = ref currentGameState.GetRegion(myRegionId);
                var neighbours = currentGameState.GetNeighbourRegions(myRegionId)
                    .Where(x => x.OwnerId != currentGameState.PlayerId)
                    .OrderByDescending(x => RegionMinEvaluator.GetValue(currentGameState, x));

                foreach (RegionMin neighbour in neighbours)
                {
                    if (neighbour.Army < myRegion.Army)
                    {
                        botAttacks.Add(new BotAttack(myRegion.OwnerId, myRegionId, myRegion.Army - 1, neighbour.Id));
                        myRegion.Army = 1;
                    }
                }
            }
        }
    }
}