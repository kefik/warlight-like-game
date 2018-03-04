namespace GameAi.BotStructures.MCTS.ActionGenerators
{
    using EvaluationStructures;
    using GameRecording;
    using Interfaces;

    /// <summary>
    /// Represents action generator for bot that always plays aggressively.
    /// </summary>
    internal class AggressiveBotActionGenerator : IGameActionGenerator<BotGameTurn, PlayerPerspective>
    {
        public BotGameTurn Generate(PlayerPerspective currentGameState)
        {
            throw new System.NotImplementedException();
        }
    }
}