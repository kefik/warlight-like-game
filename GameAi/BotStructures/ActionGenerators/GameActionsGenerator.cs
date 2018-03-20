namespace GameAi.BotStructures.ActionGenerators
{
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

        protected IEnumerable<BotGameTurn> DeployOffensively(ref PlayerPerspective currentGameState)
        {
            var botGameTurns = new List<BotGameTurn>();
            var playerPerspective = currentGameState;

            // neighbours of regions that are valuable and should be attacked
            var regionsToDeploy = (from region in playerPerspective.GetMyRegions()
                                   from neighbour in region.NeighbourRegionsIds
                                       .Select(x => playerPerspective.GetRegion(x))
                                   where neighbour.OwnerId != playerPerspective.PlayerId
                                   orderby RegionMinEvaluator.GetValue(playerPerspective, neighbour) descending
                                   select region).Distinct().ToList();

            int unitsToDeploy = currentGameState.GetMyIncome();
            foreach (RegionMin regionMin in regionsToDeploy)
            {
                botGameTurns.Add(new BotGameTurn(currentGameState.PlayerId)
                {
                    Deployments = new List<BotDeployment>()
                    {
                        new BotDeployment(regionMin.Id, regionMin.Army + unitsToDeploy, currentGameState.PlayerId)
                    }
                });
            }

            return botGameTurns;
        }

        protected IEnumerable<BotGameTurn> DeployDefensively(ref PlayerPerspective currentGameState)
        {
            var botGameTurns = new List<BotGameTurn>();
            var playerPerspective = currentGameState;

            // valuable regions that should be defended
            var regionsToDeploy = from region in playerPerspective.GetMyRegions()
                                  orderby RegionMinEvaluator.GetValue(playerPerspective, region) descending 
                                  select region;

            int unitsToDeploy = currentGameState.GetMyIncome();
            foreach (RegionMin regionMin in regionsToDeploy)
            {
                botGameTurns.Add(new BotGameTurn(currentGameState.PlayerId)
                {
                    Deployments = new List<BotDeployment>()
                    {
                        new BotDeployment(regionMin.Id, regionMin.Army + unitsToDeploy, currentGameState.PlayerId)
                    }
                });
            }

            return botGameTurns;
        }

        //protected IEnumerable<BotGameTurn> AttackNonAggressively(ref PlayerPerspective currentGameState)
        //{
            
        //}
    }
}