namespace GameAi.BotStructures.MCTS
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.Evaluators;
    using Interfaces.Evaluators.StructureEvaluators;

    internal class MinimaxCalculator
    {
        private readonly IRoundEvaluator roundEvaluator;
        private readonly IPlayerPerspectiveEvaluator playerPerspectiveEvaluator;

        public MinimaxCalculator(IRoundEvaluator roundEvaluator, IPlayerPerspectiveEvaluator playerPerspectiveEvaluator)
        {
            this.roundEvaluator = roundEvaluator;
            this.playerPerspectiveEvaluator = playerPerspectiveEvaluator;
        }

        public IReadOnlyList<NodeState> CalculateBestActions(
            MapMin map,
            byte myPlayerId,
            byte enemyPlayerId,
            IEnumerable<BotTurn> myBotTurns,
            IEnumerable<BotTurn> enemyBotTurns)
        {
            List<(MapMin Map, BotTurn Turn, double Value)> results
                = new List<(MapMin Map, BotTurn Turn, double Value)>();

            enemyBotTurns = enemyBotTurns.ToList();

            // resulting maps after playing these options
            foreach (var myBotTurn in myBotTurns)
            {
                // enemy tries to minimize this value
                // represents best turn for enemy
                double bestResultForEnemy = double.MaxValue;
                MapMin bestResultForEnemyMap = default(MapMin);
                foreach (var enemyBotTurn in enemyBotTurns)
                {
                    var round = new BotRound()
                    {
                        BotTurns = new[]
                        {
                            myBotTurn, enemyBotTurn
                        }
                    };

                    var newMap = roundEvaluator.Evaluate(map,
                        round);

                    var myPerspectiveValue = playerPerspectiveEvaluator
                        .GetValue(new PlayerPerspective(newMap, myPlayerId));
                    var enemyPerspectiveValue = playerPerspectiveEvaluator
                        .GetValue(new PlayerPerspective(newMap, enemyPlayerId));

                    // enemy wants to minimize this value
                    double result = myPerspectiveValue - enemyPerspectiveValue;

                    if (bestResultForEnemy > result)
                    {
                        bestResultForEnemy = result;
                        bestResultForEnemyMap = newMap;
                    }
                }

                results.Add((bestResultForEnemyMap, myBotTurn, bestResultForEnemy));
            }

            return results.OrderByDescending(x => x.Value).Select(x => new NodeState()
            {
                BoardState = x.Map,
                BotTurn = x.Turn
            }).ToList();
        }
    }
}