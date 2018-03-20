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

        protected void UpdateGameStateAfterDeploying(ref PlayerPerspective gameState, ICollection<(int RegionId, int Army, int DeployingPlayerId)> deployments)
        {
            foreach (var (regionId, army, deployingPlayerId) in deployments)
            {
                ref var region = ref gameState.GetRegion(regionId);
                region.Army = army;
            }
        }

        protected void AppendRedistributeInlandArmy(PlayerPerspective playerPerspective,
            ICollection<(int AttackingPlayerId, int AttackingRegionId,
                int AttackingArmy, int DefendingRegionId)> attacks)
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
                attacks.Add((playerPerspective.PlayerId,
                    regionMin.Id, armyToBeMoved, regionIdToMoveTo));

                // move the units
                ref var regionMinRef = ref playerPerspective.GetRegion(regionMin.Id);
                ref var regionToMoveTo = ref playerPerspective.GetRegion(regionIdToMoveTo);

                regionToMoveTo.Army += armyToBeMoved;
                regionMinRef.Army -= armyToBeMoved;
            }
        }

        protected void DeployOffensively(ref PlayerPerspective currentGameState)
        {
            var playerPerspective = currentGameState;

            var regionsToDeploy = (from region in playerPerspective.GetMyRegions()
                                   from neighbour in region.NeighbourRegionsIds
                                       .Select(x => playerPerspective.GetRegion(x))
                                   where neighbour.OwnerId != playerPerspective.PlayerId
                                   orderby RegionMinEvaluator.GetValue(playerPerspective, neighbour) descending
                                   select region.Id).Distinct();
        }
    }
}