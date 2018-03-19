namespace GameAi.BotStructures.ActionGenerators
{
    using System.Collections.Generic;
    using Data.EvaluationStructures;

    public abstract class GameActionsGenerator
    {
        protected void UpdateGameStateAfterDeploying(ref PlayerPerspective gameState, ICollection<(int RegionId, int Army, int DeployingPlayerId)> deployments)
        {
            foreach (var (regionId, army, deployingPlayerId) in deployments)
            {
                ref var region = ref gameState.GetRegion(regionId);
                region.Army = army;
            }
        }
    }
}