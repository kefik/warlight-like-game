namespace GameAi.BotStructures.ActionGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.Evaluators.StructureEvaluators;

    public abstract class GameActionsGenerator
    {
        protected DistanceMatrix DistanceMatrix;
        protected readonly IRegionMinEvaluator RegionMinEvaluator;

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
        /// Deploy near valuable not owned Regions.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        /// <remarks>
        /// Each instance of return value represents one way to deploy. Deploying is done only by full available units.
        /// </remarks>
        protected IEnumerable<BotDeployment> DeployOffensively(ref PlayerPerspective currentGameState)
        {
            var botDeployment = new List<BotDeployment>();
            var playerPerspective = currentGameState;

            // neighbours of regions that are valuable and should be attacked
            var regionsToDeploy = (from region in playerPerspective.GetMyRegions()
                                   from neighbour in region.NeighbourRegionsIds
                                       .Select(x => playerPerspective.GetRegion(x))
                                   where neighbour.OwnerId != playerPerspective.PlayerId
                                   orderby RegionMinEvaluator.GetValue(playerPerspective, neighbour) descending
                                   select region).Distinct().Take(5).ToList();

            int unitsToDeploy = currentGameState.GetMyIncome();
            foreach (RegionMin regionMin in regionsToDeploy)
            {
                botDeployment.Add(
                    new BotDeployment(regionMin.Id, regionMin.Army + unitsToDeploy, currentGameState.PlayerId));
            }

            return botDeployment;
        }

        /// <summary>
        /// Deploy in my valuable Regions.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        protected IEnumerable<BotDeployment> DeployDefensively(ref PlayerPerspective currentGameState)
        {
            var botDeployments = new List<BotDeployment>();
            var playerPerspective = currentGameState;

            // valuable regions that should be defended
            var regionsToDeploy = (from region in playerPerspective.GetMyRegions()
                                   orderby RegionMinEvaluator.GetValue(playerPerspective, region) descending
                                   select region).Take(5);

            int unitsToDeploy = currentGameState.GetMyIncome();
            foreach (RegionMin regionMin in regionsToDeploy)
            {
                botDeployments.Add(
                        new BotDeployment(regionMin.Id, regionMin.Army + unitsToDeploy, currentGameState.PlayerId));
            }

            return botDeployments;
        }

        /// <summary>
        /// Deploy in regions near enemy strong force.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        protected IEnumerable<BotDeployment> DeployToCounterEnemy(ref PlayerPerspective currentGameState)
        {
            var botDeployments = new List<BotDeployment>();

            var playerPerspective = currentGameState;
            var myRegions = currentGameState.GetMyRegions();

            var regionsCloseToEnemy = myRegions
                .Select(x => new
                {
                    MyRegion = x,
                    EnemyRegion = playerPerspective.GetClosestRegion(x,
                        y => y.OwnerId != playerPerspective.PlayerId && y.OwnerId != 0)
                })
                .Select(x => new
                {
                    x.MyRegion,
                    x.EnemyRegion,
                    Distance = DistanceMatrix.GetDistance(x.MyRegion.Id, x.EnemyRegion.Id)
                })
            .Where(x => x.Distance < 3)
            .Where(x => x.EnemyRegion.Army > x.MyRegion.Army * 2)
            .OrderBy(x => x.Distance);

            foreach (var pairs in regionsCloseToEnemy)
            {
                botDeployments.Add(
                        new BotDeployment(pairs.MyRegion.Id, pairs.MyRegion.Army + playerPerspective.GetMyIncome(), currentGameState.PlayerId));
            }

            return botDeployments;
        }

        /// <summary>
        /// Generate all kinds of deployments.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        protected IEnumerable<BotDeployment> GenerateDeployments(PlayerPerspective currentGameState)
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
        protected IEnumerable<BotAttack> AttackSafely(PlayerPerspective currentGameState)
        {
            var botAttacks = new List<BotAttack>();

            foreach (var regionMin in currentGameState.GetMyRegions())
            {
                ref var refRegionMin = ref currentGameState.GetRegion(regionMin.Id);
                var neighboursToAttack = regionMin.NeighbourRegionsIds
                    .Select(x => currentGameState.GetRegion(x))
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
                    if (regionMin.Army - 1 > neighbourToAttack.Army * 10d / 9
                        && !neighboursNeighbours.Any(x => x.OwnerId != 0 && x.OwnerId != currentGameState.PlayerId))
                    {
                        // don't attack with too large force
                        int attackingArmy = Math.Min(neighbourToAttack.Army * 3, regionMin.Army - 1);

                        botAttacks.Add(new BotAttack(currentGameState.PlayerId, refRegionMin.Id,
                            attackingArmy, neighbourToAttack.Id));

                        refRegionMin.Army -= attackingArmy;
                    }
                }
            }

            return botAttacks;
        }

        protected IEnumerable<BotAttack> AttackAggressively(PlayerPerspective currentGameState)
        {
            return null;
        }
    }
}