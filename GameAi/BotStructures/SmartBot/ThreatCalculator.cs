namespace GameAi.BotStructures.SmartBot
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;

    internal class ThreatCalculator
    {
        private byte[] playersIds;

        public ThreatCalculator(byte[] playersIds)
        {
            this.playersIds = playersIds;
        }

        public IList<ThreatStructure> EvaluateThreats(
            PlayerPerspective playerPerspective)
        {
            var enemyPerspective =
                new PlayerPerspective(playerPerspective.MapMin,
                    playersIds.First(
                        x => x != playerPerspective.PlayerId));

            int enemyIncome = enemyPerspective.GetMyIncome();

            var list = new List<ThreatStructure>();

            foreach (var regionMin in playerPerspective.GetMyRegions()
            )
            {
                var enemyNeighbours = playerPerspective
                    .GetNeighbourRegions(regionMin)
                    // region of enemy
                    .Where(x => enemyPerspective.IsRegionMine(x))
                    .ToList();

                // no enemy regions => ignore
                if (enemyNeighbours.Count == 0)
                {
                    continue;
                }

                int threatArmy = enemyNeighbours.Sum(x => x.Army - 1);
                int fullDeploymentThreatArmy =
                    threatArmy + enemyIncome;

                // attack doesnt kill enough my units
                if (fullDeploymentThreatArmy < regionMin.Army)
                {
                    continue;
                }

                list.Add(new ThreatStructure()
                {
                    MyRegionId = regionMin.Id,
                    ThreatArmy = threatArmy,
                    ThreatArmyWithFullDeployment =
                        fullDeploymentThreatArmy
                });
            }

            return list;
        }
    }

    /// <summary>
    /// Represents information about threat that can come from the enemy.
    /// </summary>
    internal class ThreatStructure
    {
        /// <summary>
        /// ID of my region that is under threat.
        /// </summary>
        public int MyRegionId { get; set; }

        /// <summary>
        /// Army that is threatening my region
        /// including full deployment of units of enemy.
        /// </summary>
        public int ThreatArmyWithFullDeployment { get; set; }

        /// <summary>
        /// Army threatening to conquer my region.
        /// </summary>
        public int ThreatArmy { get; set; }
    }
}