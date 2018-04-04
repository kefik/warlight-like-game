namespace GameAi.BotStructures.SmartBot
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using ActionGenerators;
    using Common.Extensions;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.StructureEvaluators;

    internal class MCTSSmartBotActionsGenerator
        : IGameActionsGenerator
    {
        private IRegionMinEvaluator regionMinEvaluator;
        private readonly ThreatCalculator threatCalculator;
        private readonly ISuperRegionMinEvaluator superRegionMinEvaluator;
        private byte[] playersIds;
        private Random random;

        public MCTSSmartBotActionsGenerator(
            DistanceMatrix distanceMatrix,
            IRegionMinEvaluator regionMinEvaluator,
            ISuperRegionMinEvaluator superRegionMinEvaluator,
            byte[] playersIds)
        {
            this.regionMinEvaluator = regionMinEvaluator;
            this.superRegionMinEvaluator = superRegionMinEvaluator;
            this.playersIds = playersIds;
            threatCalculator = new ThreatCalculator(playersIds);
        }

        public IReadOnlyList<BotGameTurn> Generate(
            PlayerPerspective currentGameState)
        {
            currentGameState = currentGameState.ShallowCopy();

            var enemyPerspective =
                new PlayerPerspective(currentGameState.MapMin,
                    playersIds.First(
                        x => x != currentGameState.PlayerId));

            var threatsToMe =
                threatCalculator.EvaluateThreats(currentGameState);
            
            var threatsToEnemy =
                threatCalculator.EvaluateThreats(enemyPerspective)
                .OrderBy(x => x.SpoilsBonus);

            // TODO: deploy and reinforce to counter enemy threats
            var groupedThreatsToMe =
                threatsToEnemy.GroupBy(x => x.SuperRegionId);



            return null;
        }

        private (IEnumerable<BotDeployment> Deployments,
            IEnumerable<BotAttack> Attacks) AttackOnEnemy(
                PlayerPerspective playerPerspective,
                IEnumerable<Threat> threatsToEnemy)
        {
            var groupedThreats = threatsToEnemy
                .GroupBy(
                    threat => new
                    {
                        threat.SuperRegionId,
                        threat.SpoilsBonus
                    },
                    threat => threat)
                // first those group that spoil the region
                .OrderByDescending(
                    x => x.FirstOrDefault()?.SpoilsBonus)
                // sort by super region with highest value
                .ThenByDescending(
                    x => superRegionMinEvaluator.GetValue(
                        playerPerspective,
                        playerPerspective.GetSuperRegion(x.Key
                            .SuperRegionId)));

            foreach (var groupedThreat in groupedThreats)
            {
                List<Threat> threats = groupedThreat.ToList();

                // spoils bonus and more than one regions threat
                if (threats.Count > 1 && groupedThreat.Key.SpoilsBonus)
                {
                    threats = threats.OrderByDescending(x => x
                        .GetMinimumNeededArmyToDefendFullDeployment(
                            playerPerspective.MapMin))
                            .ToList();
                }
            }

            return (null, null);
        }

        private IEnumerable<BotAttack> ReinforceToCounterThreats(
            PlayerPerspective playerPerspective, 
            IEnumerable<Threat> threatsToMe)
        {
            var regionsUnderThreat =
                threatsToMe
                .Distinct()
                .ToHashSet(x => x.RegionId);

            var groupedThreats = threatsToMe
                .GroupBy(threat => threat.SuperRegionId,
                    threat => new
                    {
                        Threat = threat,
                        threat.SpoilsBonus
                    })
                // first those group that spoil the region
                .OrderByDescending(
                    x => x.FirstOrDefault()?.SpoilsBonus)
                // sort by super region with highest value
                .ThenByDescending(
                    x => superRegionMinEvaluator.GetValue(
                        playerPerspective,
                        playerPerspective.GetSuperRegion(x.Key)))
                .ToLookup(x => x.Key);

            /* TODO: try to reinforce from regions of super region
            that wont be spoiled or region not neighbouring to enemy */
            foreach (var groupedThreat in groupedThreats)
            {
                var threats = groupedThreat.ToList();
            }

            return null;
        }
       
    }
}