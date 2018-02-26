namespace GameAi
{
    /// <summary>
    /// Evaluation state of the bot.
    /// </summary>
    public enum BotEvaluationState
    {
        /// <summary>
        /// Bot is not running at the moment.
        /// </summary>
        NotRunning,
        /// <summary>
        /// Bot is running at the moment.
        /// </summary>
        Running,
        /// <summary>
        /// Bot should stop.
        /// </summary>
        ShouldStop
    }
}