namespace GameAi.BotStructures.MCTS
{
    using Data.EvaluationStructures;

    /// <summary>
    /// Represents state of the board.
    /// </summary>
    internal class NodeState
    {
        public int VisitCount { get; set; }
        public int WinCount { get; set; }

        public PlayerPerspective BoardState { get; set; }
    }
}