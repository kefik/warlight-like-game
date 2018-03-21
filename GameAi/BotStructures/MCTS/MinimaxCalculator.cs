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

        public IReadOnlyList<BoardEvaluationResult> CalculateBestActions(
            MapMin map,
            byte myPlayerId,
            byte enemyPlayerId,
            IEnumerable<BotTurn> myBotTurns,
            IEnumerable<BotTurn> enemyBotTurns)
        {
            List<BoardEvaluationResult> results
                = new List<BoardEvaluationResult>();

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

                    // get expected result
                    (MapMin newMap, double result) = GetExpectedValueResult(map, round, myPlayerId, enemyPlayerId);

                    if (bestResultForEnemy > result)
                    {
                        bestResultForEnemy = result;
                        bestResultForEnemyMap = newMap;
                    }
                }

                results.Add(
                    new BoardEvaluationResult()
                    {
                        BoardState = bestResultForEnemyMap,
                        BotTurn = myBotTurn,
                        Result = bestResultForEnemy
                    });
            }

            return results.OrderByDescending(x => x.Result).ToList();
        }

        private (MapMin MapMin, double Value) GetExpectedValueResult(MapMin previousMap, BotRound round,
            byte myPlayerId, byte enemyPlayerId)
        {
            var boardEvaluationResults = new List<(MapMin ResultMap, double Value)>();

            // should be iterated only in case of non-deterministic algorithm
            for (int i = 0; i < 20; i++)
            {
                var newMap = roundEvaluator.Evaluate(previousMap,
                    round);
                var myPerspectiveValue = playerPerspectiveEvaluator
                    .GetValue(new PlayerPerspective(newMap, myPlayerId));
                var enemyPerspectiveValue = playerPerspectiveEvaluator
                    .GetValue(new PlayerPerspective(newMap, enemyPlayerId));

                boardEvaluationResults.Add((newMap, myPerspectiveValue - enemyPerspectiveValue));
            }

            boardEvaluationResults = boardEvaluationResults.OrderBy(x => x.Value).ToList();

            return boardEvaluationResults[boardEvaluationResults.Count / 2];
        }
    }
}