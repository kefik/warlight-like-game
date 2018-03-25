namespace GameAi
{
    using System.Collections.Generic;
    using System.Linq;
    using BotStructures.MCTS;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.Evaluators;
    using Interfaces.Evaluators.StructureEvaluators;

    internal class ProbabilityAwareRoundEvaluator
    {
        private IRoundEvaluator roundEvaluator;
        private IPlayerPerspectiveEvaluator playerPerspectiveEvaluator;
        private byte myPlayerId;
        private byte enemyPlayerId;

        public ProbabilityAwareRoundEvaluator(
            IRoundEvaluator roundEvaluator,
            IPlayerPerspectiveEvaluator playerPerspectiveEvaluator,
            byte myPlayerId, byte enemyPlayerId)
        {
            this.roundEvaluator = roundEvaluator;
            this.playerPerspectiveEvaluator = playerPerspectiveEvaluator;
            this.myPlayerId = myPlayerId;
            this.enemyPlayerId = enemyPlayerId;
        }

        public MapMin EvaluateInExpectedValue(MapMin mapMin, BotRound round)
        {
            var list = GetSamples(mapMin, round);

            // take median
            return list[list.Count / 2].MapMin;
        }

        public MapMin EvaluateInWorstCase(MapMin mapMin, BotRound round)
        {
            var list = GetSamples(mapMin, round);

            // take worst value
            return list[0].MapMin;
        }

        public (MapMin Expected, MapMin WorstCase) EvaluateInExpectedAndWorstCase(MapMin mapMin, BotRound round)
        {
            var list = GetSamples(mapMin, round);

            return (list[list.Count / 2].MapMin, list[0].MapMin);
        }

        public MapMin EvaluateInRandomCase(MapMin mapMin, BotRound round)
        {
            return roundEvaluator.Evaluate(mapMin, round);
        }

        private List<(MapMin MapMin, double Value)> GetSamples(MapMin mapMin, BotRound round)
        {
            var list = new List<(MapMin MapMin, double Value)>();
            for (int i = 0; i < 20; i++)
            {
                var resultMapMin = roundEvaluator.Evaluate(mapMin, round);

                var myPlayerPerspective = new PlayerPerspective(resultMapMin, myPlayerId);
                var enemyPlayerPerspective = new PlayerPerspective(resultMapMin, enemyPlayerId);

                double positionValue = playerPerspectiveEvaluator.GetValue(myPlayerPerspective)
                                       - playerPerspectiveEvaluator.GetValue(enemyPlayerPerspective);

                list.Add((resultMapMin, positionValue));
            }
            list = list.OrderBy(x => x.Value).ToList();
            return list;
        }
    }
}