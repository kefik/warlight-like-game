namespace GameAi.BotStructures.MCTS.ActionGenerators
{
    using System;
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

            var deploying = GenerateDeploying(currentState);

            // TODO: update game state

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
            throw new NotImplementedException();
        }
    }
}