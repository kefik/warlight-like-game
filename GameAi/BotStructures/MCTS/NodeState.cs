namespace GameAi.BotStructures.MCTS
{
    using Data.EvaluationStructures;
    using Data.GameRecording;

    /// <summary>
    /// Represents state of the board.
    /// </summary>
    internal class NodeState
    {
        public int VisitCount { get; set; }
        public double WinCount { get; set; }

        public MapMin BoardState { get; set; }

        /// <summary>
        /// Represents turn that led to this particular <see cref="BoardState"/>.
        /// </summary>
        public BotTurn BotTurn { get; set; }
    }
}