namespace GameAi.Interfaces.ActionsGenerators
{
    using Data.EvaluationStructures;
    using Data.GameRecording;

    public interface IGameActionsGenerator
        : IActionsGenerator<BotGameTurn, PlayerPerspective>
    {
        
    }
}