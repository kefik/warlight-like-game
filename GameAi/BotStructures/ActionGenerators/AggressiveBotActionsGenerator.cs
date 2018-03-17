namespace GameAi.BotStructures.ActionGenerators
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces;
    using Interfaces.ActionsGenerators;

    /// <summary>
    /// Represents action generator for bot that always plays aggressively.
    /// </summary>
    internal class AggressiveBotActionsGenerator : IGameActionsGenerator
    {
        /// <summary>
        /// Generates bot game turn based on current state of the game.
        /// </summary>
        /// <param name="currentGameState">Current state of the game.</param>
        /// <returns></returns>
        public IReadOnlyList<BotGameTurn> Generate(PlayerPerspective currentGameState)
        {
            var currentState = currentGameState.ShallowCopy();

            // generate deploying
            var deploying = GenerateDeploying(currentState);

            // update game state after deploying
            UpdateGameStateAfterDeploying(ref currentState, deploying);

            // generate attacking
            var attacking = GenerateAttacking(currentState);

            return new List<BotGameTurn>()
            {
                new BotGameTurn(currentState.PlayerId)
                {
                    Deployments = deploying,
                    Attacks = attacking
                }
            };
        }

        private IList<(int RegionId, int Army, int DeployingPlayer)> GenerateDeploying(PlayerPerspective currentGameState)
        {
            var myRegions = currentGameState.GetMyRegions();
            int canDeployUnitsCount = currentGameState.GetMyIncome();
            
            // must have region
            var regionToDeployTo = myRegions.First();
            
            // TODO: deploy reasonably
            return new List<(int RegionId, int Army, int DeployingPlayerId)>()
            {
                (regionToDeployTo.Id, regionToDeployTo.Army + canDeployUnitsCount, currentGameState.PlayerId)
            };
        }

        private IList<(int AttackingPlayerId, int AttackingRegionId, int AttackingArmy, int DefendingRegionId)> GenerateAttacking(
            PlayerPerspective currentGameState)
        {
            IList<(int AttackingPlayerId, int AttackingRegionId, int AttackingArmy, int DefendingRegionId)> attacks
                = new List<(int AttackingPlayerId, int AttackingRegionId, int AttackingArmy, int DefendingRegionId)>();
            // my region with biggest army
            var regionsWithHighestArmy = currentGameState.GetMyRegions().OrderByDescending(x => x.Army);

            foreach (var region in regionsWithHighestArmy)
            {
                var neighbourRegions = currentGameState.GetNeighbourRegions(region.Id).OrderBy(x => x.Army);

                var neighbourToAttack =
                    neighbourRegions.FirstOrDefault(x => x.IsVisible &&
                                                         x.GetOwnerPerspective(currentGameState.PlayerId) !=
                                                         OwnerPerspective.Mine);

                // if you can attack on neighbour, attack
                if (!neighbourToAttack.Equals(default(RegionMin))
                    && region.Army > 2 && region.Army > neighbourToAttack.Army)
                {
                    int attackingArmy = region.Army - 1;

                    // attack on not mine region with lowest army
                    attacks.Add((region.OwnerId, region.Id, attackingArmy,
                        neighbourToAttack.Id));
                }
            }

            return attacks;
        }

        private void UpdateGameStateAfterDeploying(ref PlayerPerspective gameState, ICollection<(int RegionId, int Army, int DeployingPlayerId)> deployments)
        {
            foreach (var (regionId, army, deployingPlayerId) in deployments)
            {
                ref var region = ref gameState.GetRegion(regionId);
                region.Army = army;
            }
        }
    }
}