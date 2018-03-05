namespace GameAi.BotStructures.MCTS.ActionGenerators
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using EvaluationStructures;
    using Interfaces;
    using InterFormatCommunication.GameRecording;

    /// <summary>
    /// Represents action generator for bot that always plays aggressively.
    /// </summary>
    internal class AggressiveBotActionGenerator : IGameActionGenerator<BotGameTurn, PlayerPerspective>
    {
        /// <summary>
        /// Generates bot game turn based on current state of the game.
        /// </summary>
        /// <param name="currentGameState">Current state of the game.</param>
        /// <returns></returns>
        public BotGameTurn Generate(PlayerPerspective currentGameState)
        {
            var currentState = currentGameState.ShallowCopy();

            // generate deploying
            var deploying = GenerateDeploying(currentState);

            // update game state after deploying
            UpdateGameStateAfterDeploying(ref currentGameState, deploying);

            // generate attacking
            var attacking = GenerateAttacking(currentState);

            return new BotGameTurn(currentState.PlayerId)
            {
                Deployments = deploying,
                Attacks = attacking
            };
        }

        private ICollection<(int RegionId, int Army)> GenerateDeploying(PlayerPerspective currentGameState)
        {
            var myRegions = currentGameState.GetMyRegions();
            int canDeployUnitsCount = currentGameState.GetMyIncome();

            int regionToDeployToId = myRegions.First().Id;
            
            // TODO: deploy reasonably
            return new List<(int RegionId, int Army)>()
            {
                (regionToDeployToId, canDeployUnitsCount)
            };
        }

        private ICollection<(int AttackingRegionId, int AttackingArmy, int DefendingRegionId)> GenerateAttacking(
            PlayerPerspective currentGameState)
        {
            IList<(int AttackingRegionId, int AttackingArmy, int DefendingRegionId)> attacks
                = new List<(int AttackingRegionId, int AttackingArmy, int DefendingRegionId)>();
            // my region with biggest army
            var regionsWithHighestArmy = currentGameState.GetMyRegions().OrderByDescending(x => x.Army);

            foreach (var region in regionsWithHighestArmy)
            {
                var neighbourRegions = currentGameState.MapMin.GetNeighbourRegions(region.Id).OrderBy(x => x.Army);

                var neighbourToAttack =
                    neighbourRegions.FirstOrDefault(x => x.IsVisible &&
                                                         x.GetOwnerPerspective(currentGameState.PlayerId) !=
                                                         OwnerPerspective.Mine);

                int attackingArmy = region.Army - 1;
                
                // attack on not mine region with lowest army
                attacks.Add((region.Id, attackingArmy, neighbourToAttack.Id));
            }

            return attacks;
        }

        private void UpdateGameStateAfterDeploying(ref PlayerPerspective gameState, ICollection<(int RegionId, int Army)> deployments)
        {
            foreach (var (regionId, army) in deployments)
            {
                ref var region = ref gameState.MapMin.GetRegion(regionId);
                region.Army += army;
            }
        }
    }
}