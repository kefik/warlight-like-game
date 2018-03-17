namespace GameAi.Interfaces.Evaluators
{
    using Data.EvaluationStructures;
    using Data.GameRecording;

    /// <summary>
    /// Object implementing this interface can evaluate round.
    /// </summary>
    public interface IRoundEvaluator
    {
        MapMin Evaluate(MapMin map, BotRound round);
        MapMin Evaluate(MapMin map, LinearizedBotRound botRound);
    }
}