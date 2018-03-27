namespace GameAi.Interfaces
{
    using Evaluators.StructureEvaluators;

    /// <summary>
    /// Class implementing this interface can be inejcted with
    /// evaluators.
    /// </summary>
    public interface IEvaluatorInjectable
    {
        void Inject(IRegionMinEvaluator gameBeginningRegionMinEvaluator,
            ISuperRegionMinEvaluator gameBeginningSuperRegionMinEvaluator,
            IRegionMinEvaluator gameRegionMinEvaluator,
            ISuperRegionMinEvaluator gameSuperRegionMinEvaluator);
    }
}