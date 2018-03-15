namespace GameAi.Interfaces.Evaluators.StructureEvaluators
{
    using Data.EvaluationStructures;

    public interface IPlayerPerspectiveEvaluator
    {
        double GetValue(PlayerPerspective playerPerspective);
    }
}