namespace GameAi.Interfaces.ActionsGenerators
{
    using Data.EvaluationStructures;
    using Data.GameRecording;

    public interface IGameBeginningActionsGenerator
        : IActionsGenerator<BotGameBeginningTurn, PlayerPerspective>
    {
        
    }
}