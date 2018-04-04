namespace GameAi.BotStructures.SmartBot
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;

    /// <summary>
    /// Component handling evaluating threats
    /// to specified player.
    /// </summary>
    internal class ThreatCalculator
    {
        private byte[] playersIds;

        /// <summary>
        /// Constructs <see cref="ThreatCalculator"/>.
        /// </summary>
        /// <param name="playersIds">IDs of all players.</param>
        public ThreatCalculator(byte[] playersIds)
        {
            this.playersIds = playersIds;
        }

        /// <summary>
        /// Evaluates threat.
        /// </summary>
        /// <param name="playerPerspective"></param>
        /// <returns>Returns threats to all regions of the player.</returns>
        public IList<Threat> EvaluateThreats(
            PlayerPerspective playerPerspective)
        {
            var enemyPerspective =
                new PlayerPerspective(playerPerspective.MapMin,
                    playersIds.First(
                        x => x != playerPerspective.PlayerId));

            int enemyIncome = enemyPerspective.GetMyIncome();

            var list = new List<Threat>();

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

                var threat =
                    new Threat()
                    {
                        RegionId = regionMin.Id,
                        SuperRegionId = regionMin.SuperRegionId,
                        ThreatArmy = threatArmy,
                        ThreatArmyWithFullDeployment =
                            fullDeploymentThreatArmy
                    };

                ref var superRegion =
                    ref playerPerspective.GetSuperRegion(regionMin
                        .SuperRegionId);

                // if I own the super region => it spoils the bonus
                if (superRegion.OwnerId == playerPerspective.PlayerId)
                {
                    threat.SpoilsBonus = true;
                }

                list.Add(threat);
            }

            return list;
        }
    }
}