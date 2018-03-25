namespace GameAi.BotStructures.StructuresEvaluators
{
    using Data.EvaluationStructures;
    using Interfaces.Evaluators.StructureEvaluators;

    internal class PlayerPerspectiveEvaluator : IPlayerPerspectiveEvaluator
    {
        private readonly IRegionMinEvaluator regionMinEvaluator;

        private const double ArmyCoefficient = 20;

        public PlayerPerspectiveEvaluator(IRegionMinEvaluator regionMinEvaluator)
        {
            this.regionMinEvaluator = regionMinEvaluator;
        }

        /// <summary>
        /// Obtains value defining quality of
        /// <see cref="PlayerPerspective"/> current state.
        /// Greater the value is, the better players position is.
        /// </summary>
        /// <param name="playerPerspective"></param>
        /// <returns></returns>
        public double GetValue(PlayerPerspective playerPerspective)
        {
            double value = 0;

            // sum the regions you have
            foreach (RegionMin regionMin in playerPerspective.GetMyRegions())
            {
                value += regionMinEvaluator.GetValue(playerPerspective, regionMin);
                value += regionMin.Army * ArmyCoefficient;
            }

            return value;
        }
    }
}