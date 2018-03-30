namespace GameAi.BotStructures.ActionGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.Evaluators.StructureEvaluators;

    /// <summary>
    /// Class that serves as list of functions any
    /// actions generator inheriting from it can choose
    /// from and combine.
    /// </summary>
    internal abstract class GameActionsGenerator
    {
        protected DistanceMatrix DistanceMatrix;

        protected internal readonly IRegionMinEvaluator
            RegionMinEvaluator;

        protected GameActionsGenerator(DistanceMatrix distanceMatrix,
            IRegionMinEvaluator regionMinEvaluator)
        {
            DistanceMatrix = distanceMatrix;
            RegionMinEvaluator = regionMinEvaluator;
        }

        protected void UpdateGameStateAfterDeploying(
            ref PlayerPerspective gameState,
            ICollection<BotDeployment> deployments)
        {
            foreach (BotDeployment deployment in deployments)
            {
                ref RegionMin region =
                    ref gameState.GetRegion(deployment.RegionId);
                region.Army = deployment.Army;
            }
        }

        protected void UpdateGameStateAfterAttack(
            ref PlayerPerspective gameState,
            ICollection<BotAttack> attacks)
        {
            foreach (BotAttack attack in attacks)
            {
                ref RegionMin attackingRegion =
                    ref gameState.GetRegion(attack.AttackingArmy);
                attackingRegion.Army -= attack.AttackingArmy;
            }
        }

        protected void AppendRedistributeInlandArmy(
            PlayerPerspective playerPerspective,
            ICollection<BotAttack> attacks)
        {
            // hint: move army from inland territory to the edge of the area
            // get those regions
            IEnumerable<RegionMin> inlandRegionsWithArmy =
                from regionMin in playerPerspective.MapMin.RegionsMin
                where regionMin.Army > 1 && regionMin.OwnerId ==
                      playerPerspective.PlayerId
                where regionMin.NeighbourRegionsIds.All(
                    x => playerPerspective.GetRegion(x).OwnerId ==
                         playerPerspective.PlayerId)
                select regionMin;
            // for each inland region with army
            foreach (RegionMin regionMin in inlandRegionsWithArmy)
            {
                // get closest region that is not mine
                RegionMin closestRegion =
                    playerPerspective.GetClosestRegion(regionMin,
                        x => x.OwnerId != playerPerspective.PlayerId);

                // get path
                IEnumerable<int> path =
                    DistanceMatrix.GetPath(regionMin, closestRegion);
                // get first region of that path
                int regionIdToMoveTo = path.Skip(1).First();

                int armyToBeMoved = regionMin.Army - 1;
                // add to attacks
                attacks.Add(new BotAttack(playerPerspective.PlayerId,
                    regionMin.Id, armyToBeMoved, regionIdToMoveTo));

                // move the units
                ref RegionMin regionMinRef =
                    ref playerPerspective.GetRegion(regionMin.Id);

                regionMinRef.Army -= armyToBeMoved;
            }
        }

        /// <summary>
        ///     Deploy near valuable Regions.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Each instance of return value represents one way to deploy. Deploying is done only by full available units.
        /// </remarks>
        protected IList<BotDeployment> DeployOffensively(
            PlayerPerspective currentGameState)
        {
            var botDeployment = new List<BotDeployment>();
            PlayerPerspective playerPerspective = currentGameState;

            // neighbours of regions that are valuable and should be attacked
            IEnumerable<RegionMin> regionsToDeploy =
            from region in playerPerspective.GetMyRegions()
            let neighbours =
            region.NeighbourRegionsIds.Select(x => playerPerspective
                .GetRegion(x))
                // neighbours that are not mine
                .Where(x => x.OwnerId != currentGameState.PlayerId)
            // region has not owned neighbour
            where neighbours.Any()
            // order by regions neighbours max value
            orderby neighbours.Max(
                x => RegionMinEvaluator.GetValue(playerPerspective,
                    x)) descending
            select region;
            
            int unitsToDeploy = currentGameState.GetMyIncome();
            foreach (RegionMin regionMin in
                regionsToDeploy.Take(1))
            {
                botDeployment.Add(new BotDeployment(regionMin.Id,
                    regionMin.Army + unitsToDeploy,
                    currentGameState.PlayerId));
            }

            return botDeployment;
        }

        /// <summary>
        ///     Deploy in regions near enemy strong force.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        protected IList<BotDeployment> DeployToCounterSecurityThreat(
            PlayerPerspective currentGameState)
        {
            var botDeployments = new List<BotDeployment>();

            PlayerPerspective playerPerspective = currentGameState;
            IEnumerable<RegionMin> myRegions =
                currentGameState.GetMyRegions();

            // get my regions that have enemy army near them
            IEnumerable<RegionMin> regionsBySecurityThreat =
                from region in myRegions
                let enemyNeighbours =
                    region.NeighbourRegionsIds.Select(
                      x => playerPerspective.GetRegion(x))
                    // neighbours that belong to enemy
                    .Where(x => x.OwnerId != playerPerspective.PlayerId && x.OwnerId != 0)
                // has enemy neighbours
                where enemyNeighbours.Any()
                // army that can attack on me
                // TODO: need enemy perspective
                let threatArmy = enemyNeighbours
                    .Sum(x => x.Army - 1) + playerPerspective.GetMyIncome()
                // region is under enemy threat
                where threatArmy * 0.6 >= region.Army
                // order by region to defend value
                orderby RegionMinEvaluator.GetValue(playerPerspective, region) descending,
                    region.Army
                select region;

            // deploy
            foreach (RegionMin region in regionsBySecurityThreat.Take(
                1))
            {
                botDeployments.Add(new BotDeployment(region.Id,
                    region.Army + playerPerspective.GetMyIncome(),
                    currentGameState.PlayerId));
            }

            return botDeployments;
        }

        /// <summary>
        ///     Generate all kinds of deployments.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        /// <remarks>Using this can be resource-exhausting due to many deployment options.</remarks>
        protected IEnumerable<BotDeployment> DeployToAll(
            PlayerPerspective currentGameState)
        {
            foreach (RegionMin regionMin in currentGameState
                .GetMyRegions())
            {
                yield return new BotDeployment(regionMin.Id,
                    regionMin.Army + currentGameState.GetMyIncome(),
                    currentGameState.PlayerId);
            }
        }

        /// <summary>
        ///     Attacks only when there's no enemy around and the bot is sure it will win.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        protected void AttackNeutralSafely(
            PlayerPerspective currentGameState,
            IList<BotAttack> botAttacks)
        {
            foreach (RegionMin regionMin in currentGameState
                .GetMyRegions())
            {
                ref RegionMin refRegionMin =
                    ref currentGameState.GetRegion(regionMin.Id);
                List<RegionMin> neighboursToAttack =
                    refRegionMin.NeighbourRegionsIds
                        .Select(x => currentGameState.GetRegion(x))
                        .Where(
                            x => x.OwnerId !=
                                 currentGameState.PlayerId)
                        .OrderByDescending(x => RegionMinEvaluator
                            .GetValue(currentGameState, x))
                            .ThenBy(x => x.Army)
                            .ToList();

                // if theres any neighbour where is the enemy, don't attack (we want to attack safely only)
                if (neighboursToAttack.Any(
                    x => x.OwnerId != 0 &&
                         x.OwnerId != currentGameState.PlayerId))
                {
                    continue;
                }

                for (int index = 0;
                    index < neighboursToAttack.Count;
                    index++)
                {
                    RegionMin neighbourToAttack =
                        neighboursToAttack[index];

                    // I have large enough army and
                    // theres no neighbour of neighbour that is not mine => attack
                    if ((refRegionMin.Army - 1) * 0.6 -
                        neighbourToAttack.Army * 0.7 > 0
                    /* && !neighboursNeighbours.Any(x => x.OwnerId != 0 && x.OwnerId != currentGameState.PlayerId)*/
                    )
                    {
                        int attackingArmy;

                        // its last => attack with remaining force
                        if (index == neighboursToAttack.Count - 1)
                        {
                            attackingArmy = refRegionMin.Army - 1;
                        }
                        // don't attack with too large force
                        else
                        {
                            // calculate minimum army that will surely succeed in conquering
                            // the region
                            int minArmyThatSucceeds = (int) (neighbourToAttack.Army /
                                                             0.6);
                            attackingArmy =
                                Math.Min(minArmyThatSucceeds + 4,
                                    refRegionMin.Army - 1);
                        }

                        botAttacks.Add(new BotAttack(
                            currentGameState.PlayerId,
                            refRegionMin.Id, attackingArmy,
                            neighbourToAttack.Id));

                        refRegionMin.Army -= attackingArmy;
                    }
                }
            }
        }

        protected void AttackAggressively(
            PlayerPerspective currentGameState,
            IList<BotAttack> botAttacks)
        {
            IEnumerable<int> myRegions =
                currentGameState.GetMyRegions().Select(x => x.Id);

            foreach (int myRegionId in myRegions)
            {
                ref RegionMin myRegion =
                    ref currentGameState.GetRegion(myRegionId);
                List<RegionMin> neighbours =
                    currentGameState.GetNeighbourRegions(myRegionId)
                        .Where(
                            x => x.OwnerId !=
                                 currentGameState.PlayerId)
                        .OrderByDescending(x => RegionMinEvaluator
                            .GetValue(currentGameState, x))
                            // neighbours with lowest army first
                            .ThenBy(x => x.Army).ToList();

                for (int index = 0; index < neighbours.Count; index++)
                {
                    RegionMin neighbour = neighbours[index];
                    if ((myRegion.Army - 1) * 0.6 -
                        neighbour.Army * 0.7 >= 0)
                    {
                        // calculate minimum army that will surely succeed in conquering
                        // the region
                        int minArmyThatSucceeds;
                        if (neighbour.OwnerId != 0)
                        {
                            var enemyPlayerPerspective =
                                new PlayerPerspective(
                                    currentGameState.MapMin,
                                    neighbour.OwnerId);
                            minArmyThatSucceeds =
                                (int)((neighbour.Army + enemyPlayerPerspective.GetMyIncome()) /
                                      0.6);
                        }
                        else
                        {
                            minArmyThatSucceeds =
                                (int)(neighbour.Army /
                                      0.6);
                        }

                        int attackingArmy = Math.Min(minArmyThatSucceeds + 4,
                            myRegion.Army - 1);
                        botAttacks.Add(new BotAttack(myRegion.OwnerId,
                            myRegionId, attackingArmy, neighbour.Id));
                        myRegion.Army -= attackingArmy;
                    }
                }
            }
        }
    }
}