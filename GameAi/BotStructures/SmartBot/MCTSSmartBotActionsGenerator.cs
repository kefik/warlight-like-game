namespace GameAi.BotStructures.SmartBot
{
    using System.Collections.Generic;
    using System.Linq;
    using ActionGenerators;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.StructureEvaluators;

    internal class MCTSSmartBotActionsGenerator
        : GameActionsGenerator, IGameActionsGenerator
    {
        public MCTSSmartBotActionsGenerator(
            DistanceMatrix distanceMatrix,
            IRegionMinEvaluator regionMinEvaluator) : base(
            distanceMatrix, regionMinEvaluator)
        {
        }

        public IReadOnlyList<BotGameTurn> Generate(
            PlayerPerspective currentGameState)
        {
            return null;
        }

        private IList<BotAttack> SmartAggressive(
            PlayerPerspective currentGameState)
        {
            var attacks = new List<BotAttack>();

            // enemies ordered by army
            var enemyRegions = currentGameState.MapMin.RegionsMin
                .Where(x => x.OwnerId != 0 &&
                            x.OwnerId != currentGameState.PlayerId)
                .OrderByDescending(x => x.Army);

            foreach (var enemyRegion in enemyRegions)
            {
                var myNeighbours = enemyRegion.NeighbourRegionsIds
                    .Select(x => currentGameState.GetRegion(x))
                    .Where(
                        x => x.OwnerId == currentGameState.PlayerId)
                    .ToList();

            }
            

            return attacks;
        }

        private IList<RegionMin> GetRegionsIMustDefend(
            PlayerPerspective playerPerspective,
            IEnumerable<ThreatStructure> threats)
        {
            foreach (var threatStructure in threats)
            {
                ref var region =
                    ref playerPerspective.GetRegion(threatStructure
                        .MyRegionId);
                ref var superRegion =
                    ref playerPerspective.GetSuperRegion(region
                        .SuperRegionId);
            }

            return null;
        }
    }
}