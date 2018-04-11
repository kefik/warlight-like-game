namespace GameAi.BotStructures.ActionGenerators
{
    using Data.EvaluationStructures;

    /// <summary>
    /// Represents information about threat that can come from the enemy.
    /// </summary>
    internal class Threat
    {
        /// <summary>
        /// ID of region that is under threat.
        /// </summary>
        public int RegionId { get; set; }

        public int SuperRegionId { get; set; }

        /// <summary>
        /// Army that is threatening my region
        /// including full deployment of units of enemy.
        /// </summary>
        public int ThreatArmyWithFullDeployment { get; set; }

        /// <summary>
        /// Army threatening to conquer my region.
        /// </summary>
        public int ThreatArmy { get; set; }

        /// <summary>
        /// True, if it cancels regions owner bonus to a
        /// <see cref="SuperRegionMin"/>.
        /// </summary>
        public bool SpoilsBonus { get; set; }

        public int GetMinimumNeededArmyToDefendFullDeployment(
            MapMin mapMin)
        {
            ref var region = ref mapMin.GetRegion(RegionId);
            return (int)(ThreatArmyWithFullDeployment *
                         RoundEvaluator
                             .ProbabilityAttackingUnitKills -
                         region.Army);
        }

        public int GetMinimumNeededArmyToDefend(MapMin map)
        {
            ref var region = ref map.GetRegion(RegionId);
            return (int)(ThreatArmy *
                         RoundEvaluator
                             .ProbabilityAttackingUnitKills -
                         region.Army);
        }
    }
}