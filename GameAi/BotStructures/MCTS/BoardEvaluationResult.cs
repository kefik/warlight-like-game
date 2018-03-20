namespace GameAi.BotStructures.MCTS
{
    using Data.EvaluationStructures;
    using Data.GameRecording;

    public struct BoardEvaluationResult
    {
        public MapMin BoardState { get; set; }
        public BotTurn BotTurn { get; set; }
        public double Result { get; set; }
    }
}