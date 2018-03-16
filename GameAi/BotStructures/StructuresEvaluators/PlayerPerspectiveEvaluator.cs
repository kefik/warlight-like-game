namespace GameAi.BotStructures.StructuresEvaluators
{
    using Data.EvaluationStructures;
    using Interfaces.Evaluators.StructureEvaluators;

    internal class PlayerPerspectiveEvaluator : IPlayerPerspectiveEvaluator
    {
        private readonly IRegionMinEvaluator regionMinEvaluator;

        public PlayerPerspectiveEvaluator(IRegionMinEvaluator regionMinEvaluator)
        {
            this.regionMinEvaluator = regionMinEvaluator;
        }

        public double GetValue(PlayerPerspective playerPerspective)
        {
            double value = 0;

            // sum the regions you have

            return value;
        }
    }
}