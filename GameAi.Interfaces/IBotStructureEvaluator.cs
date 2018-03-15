namespace GameAi.Interfaces
{
    /// <summary>
    /// Component used for evaluating game structures.
    /// </summary>
    /// <typeparam name="TBotStructure"></typeparam>
    /// <typeparam name="TPlayerPerspective"></typeparam>
    public interface IBotStructureEvaluator<in TPlayerPerspective, in TBotStructure>
    {
        double GetValue(TPlayerPerspective currentGameState, TBotStructure gameStructure);
    }
}