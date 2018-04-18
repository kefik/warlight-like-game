namespace GameAi.BotStructures.ActionGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
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

        protected ISuperRegionMinEvaluator SuperRegionMinEvaluator;

        protected ThreatCalculator ThreatCalculator;

        protected GameActionsGenerator(DistanceMatrix distanceMatrix,
            IRegionMinEvaluator regionMinEvaluator,
            ISuperRegionMinEvaluator superRegionMinEvaluator,
            byte[] playersIds)
        {
            DistanceMatrix = distanceMatrix;
            RegionMinEvaluator = regionMinEvaluator;
            SuperRegionMinEvaluator = superRegionMinEvaluator;
            ThreatCalculator = new ThreatCalculator(playersIds);
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

            //// neighbours of regions that are valuable and should be attacked
            //IEnumerable<RegionMin> regionsToDeploy =
            //from region in playerPerspective.GetMyRegions()
            //let neighbours =
            //region.NeighbourRegionsIds.Select(x => playerPerspective
            //    .GetRegion(x))
            //    // neighbours that are not mine
            //    .Where(x => x.OwnerId != currentGameState.PlayerId)
            //// region has not owned neighbour
            //where neighbours.Any()
            //// order by regions neighbours max value
            //orderby neighbours.Max(
            //    x => RegionMinEvaluator.GetValue(playerPerspective,
            //        x)) descending
            //select region;

            var regionsToDeploy =
            (from region in playerPerspective.MapMin.RegionsMin
             where !playerPerspective.IsRegionMine(region)
             let neighbours = region.NeighbourRegionsIds
                 .Select(x => playerPerspective.GetRegion(x))
                 // neighbours that are mine
                 .Where(x => playerPerspective.IsRegionMine(x))
             where neighbours.Any()
             let neighboursMaxArmy = neighbours.Max(x => x.Army)
             orderby RegionMinEvaluator.GetValue(currentGameState,
                 region) descending
             // deploy to region with maximum army
             select neighbours.First(x => neighboursMaxArmy == x.Army)).Distinct();


            int unitsToDeploy = currentGameState.GetMyIncome();
            foreach (RegionMin regionMin in regionsToDeploy.Take(1))
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
            var botDeployment = new List<BotDeployment>();
            PlayerPerspective playerPerspective = currentGameState;
            int myIncome = playerPerspective.GetMyIncome();

            var threatsToMe = ThreatCalculator.EvaluateThreats(playerPerspective);

            threatsToMe = threatsToMe.OrderByDescending(x => x.SpoilsBonus)
                .ThenByDescending(
                    x => SuperRegionMinEvaluator.GetValue(
                        playerPerspective,
                        playerPerspective.GetSuperRegion(x
                            .SuperRegionId))).ToList();

            if (threatsToMe.Count == 0)
            {
                return botDeployment;
            }

            if (threatsToMe.Count == 1)
            {
                // full deploy
                var threat = threatsToMe[0];
                ref var regionToDeploy =
                    ref playerPerspective.GetRegion(threat
                        .RegionId);
                botDeployment.Add(new BotDeployment(regionToDeploy.Id,
                    regionToDeploy.Army + myIncome, playerPerspective.PlayerId));

                return botDeployment;
            }

            while (true)
            {
                foreach (var threat in threatsToMe)
                {
                    ref var region =
                        ref playerPerspective.GetRegion(threat
                            .RegionId);

                    int armyToDeploy =
                        Math.Min(myIncome,
                            threat
                                .GetMinimumNeededArmyToDefendFullDeployment(
                                    playerPerspective.MapMin) + 2);

                    if (armyToDeploy <= 0)
                    {
                        return MergeSameRegionsDeployments(botDeployment);
                    }

                    if (armyToDeploy == myIncome)
                    {
                        botDeployment.Add(new BotDeployment(region.Id,
                            region.Army + armyToDeploy,
                            playerPerspective.PlayerId));
                        return MergeSameRegionsDeployments(botDeployment);
                    }

                    botDeployment.Add(new BotDeployment(region.Id,
                        region.Army + armyToDeploy,
                        playerPerspective.PlayerId));
                    myIncome -= armyToDeploy;
                }
            }
        }

        private IList<BotDeployment> MergeSameRegionsDeployments(
            IEnumerable<BotDeployment> deployments)
        {
            var deploymentGroups = from deployment in deployments
                                   group deployment by deployment.RegionId;

            var result = deploymentGroups.Select(x => new BotDeployment(x.Key,
                x.Max(y => y.Army), x.First().DeployingPlayerId));

            return result.ToList();
        }

        protected IList<BotDeployment> DeployToExpand(
            PlayerPerspective currentGameState)
        {
            var botDeployments = new List<BotDeployment>();
            PlayerPerspective playerPerspective = currentGameState;

            // neighbours of regions that are valuable and should be attacked
            IEnumerable<RegionMin> regionsToDeploy =
                from region in playerPerspective.GetMyRegions()
                let neighbours =
                region.NeighbourRegionsIds.Select(x => playerPerspective
                        .GetRegion(x))
                    // neighbours that are not mine
                    .Where(x => x.OwnerId == 0)
                    .ToList()
                // region has not owned neighbour
                where neighbours.Any()
                // order by regions neighbours max value
                orderby neighbours.Max(
                    x => RegionMinEvaluator.GetValue(playerPerspective,
                        x)) descending,
                        neighbours.Count descending
                select region;

            // deploy
            foreach (RegionMin region in regionsToDeploy.Take(
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
        /// <param name="botAttacks"></param>
        /// <returns></returns>
        protected void AttackToExpandSafely(
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
                        // isnt mine
                        .Where(
                            x => !currentGameState.IsRegionMine(x))
                        .OrderByDescending(x => RegionMinEvaluator
                            .GetValue(currentGameState, x))
                            .ThenBy(x => x.Army)
                            .ToList();

                // if theres any neighbour where is the enemy, don't attack (we want to attack safely only)
                if (neighboursToAttack.Any(
                    currentGameState.IsRegionOfEnemy))
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
                    if ((refRegionMin.Army - 1) * RoundEvaluator.ProbabilityAttackingUnitKills -
                        neighbourToAttack.Army * RoundEvaluator.ProbabilityDefendingUnitKills > 0
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
                            int minArmyThatSucceeds = (int)(neighbourToAttack.Army /
                                                             RoundEvaluator.ProbabilityAttackingUnitKills);
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
                currentGameState.GetMyRegions()
                .Select(x => x.Id);

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
                    if ((myRegion.Army - 1) * RoundEvaluator.ProbabilityAttackingUnitKills -
                        neighbour.Army * RoundEvaluator.ProbabilityDefendingUnitKills >= 0)
                    {
                        // calculate minimum army that will surely succeed in conquering
                        // the region
                        int minArmyThatSucceeds;
                        if (index == neighbours.Count - 1)
                        {
                            minArmyThatSucceeds = myRegion.Army - 1;
                        }
                        else if (neighbour.OwnerId != 0)
                        {
                            var enemyPlayerPerspective =
                                new PlayerPerspective(
                                    currentGameState.MapMin,
                                    neighbour.OwnerId);
                            minArmyThatSucceeds =
                                (int)((neighbour.Army +
                                        enemyPlayerPerspective
                                            .GetMyIncome()) /
                                       RoundEvaluator
                                           .ProbabilityAttackingUnitKills
                                );
                        }
                        else
                        {
                            minArmyThatSucceeds =
                                (int)(neighbour.Army / RoundEvaluator
                                           .ProbabilityAttackingUnitKills
                                );
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

        protected void AttackOnEnemySafely(
            PlayerPerspective currentGameState,
            IList<BotAttack> botAttacks)
        {
            IEnumerable<int> myRegions =
                currentGameState.GetMyRegions()
                    .Select(x => x.Id);

            foreach (int myRegionId in myRegions)
            {
                ref RegionMin myRegion =
                    ref currentGameState.GetRegion(myRegionId);
                List<RegionMin> enemyNeighbours =
                    currentGameState.GetNeighbourRegions(myRegionId)
                        .Where(
                            x => x.GetOwnerPerspective(currentGameState.PlayerId) == OwnerPerspective.Enemy)
                        .ToList();

                if (enemyNeighbours.Count == 0)
                {
                    continue;
                }

                PlayerPerspective enemyPerspective =
                    new PlayerPerspective(currentGameState.MapMin,
                        enemyNeighbours[0].OwnerId);

                int enemyArmyWithFullDeployment =
                    enemyNeighbours.Sum(x => x.Army) +
                    enemyPerspective.GetMyIncome();

                // if my army is stronger than enemy's even with full deployed units
                if ((myRegion.Army - 1) * RoundEvaluator
                        .ProbabilityAttackingUnitKills -
                    enemyArmyWithFullDeployment * RoundEvaluator
                        .ProbabilityDefendingUnitKills >= 0)
                {
                    for (int index = 0; index < enemyNeighbours.Count; index++)
                    {
                        RegionMin neighbour = enemyNeighbours[index];

                        // if its good to attack enemy with full deployment
                        if ((myRegion.Army - 1) * RoundEvaluator.ProbabilityAttackingUnitKills -
                            (neighbour.Army + enemyPerspective.GetMyIncome())
                                * RoundEvaluator.ProbabilityDefendingUnitKills >= 0)
                        {
                            // calculate minimum army that will surely succeed in conquering
                            // the region
                            int minArmyThatSucceeds;
                            if (index == enemyNeighbours.Count - 1)
                            {
                                minArmyThatSucceeds = myRegion.Army - 1;
                            }
                            else
                            {
                                var enemyPlayerPerspective =
                                    new PlayerPerspective(
                                        currentGameState.MapMin,
                                        neighbour.OwnerId);
                                minArmyThatSucceeds =
                                    (int)((neighbour.Army +
                                           enemyPlayerPerspective
                                               .GetMyIncome()) /
                                          RoundEvaluator
                                              .ProbabilityAttackingUnitKills
                                    );
                            }

                            int attackingArmy = Math.Min(minArmyThatSucceeds,
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
}