namespace GameAi.BotStructures.StructuresEvaluators
{
    using Data.EvaluationStructures;
    using Interfaces.Evaluators.StructureEvaluators;

    internal class PlayerPerspectiveEvaluator : IPlayerPerspectiveEvaluator
    {
        private IRegionMinEvaluator regionMinEvaluator;

        public PlayerPerspectiveEvaluator(IRegionMinEvaluator regionMinEvaluator)
        {
            this.regionMinEvaluator = regionMinEvaluator;
        }

        public double GetValue(PlayerPerspective playerPerspective)
        {
            double value = 0;

            // sum the regions you have
            var myRegions = playerPerspective.GetMyRegions();
            foreach (RegionMin regionMin in myRegions)
            {
                value += regionMinEvaluator.GetValue(playerPerspective, regionMin);
            }

            return value;
        }
    }
}