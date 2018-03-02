namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Collections;
    using EvaluationStructures;
    using GameObjectsLib;
    using GameRecording;

    /// <summary>
    /// Bot using Monte-Carlo tree search algorithm.
    /// </summary>
    internal class MonteCarloTreeSearchBot : GameBot
    {
        private BotEvaluationState evaluationState;
        private MCTSEvaluationHandler evaluationHandler;

        public override bool CanStartEvaluation
        {
            get { return evaluationState == BotEvaluationState.NotRunning; }
        }

        public MonteCarloTreeSearchBot(PlayerPerspective playerPerspective, Difficulty difficulty, bool isFogOfWar)
            : base(playerPerspective, difficulty, isFogOfWar)
        {
        }

        public override async Task<BotTurn> FindBestMoveAsync()
        {
            if (evaluationState != BotEvaluationState.NotRunning)
            {
                throw new ArgumentException($"Cannot start evaluation if the current evaluation state is {evaluationState}");
            }

            evaluationState = BotEvaluationState.Running;

            // evaluate the tree
            evaluationHandler = new MCTSEvaluationHandler(PlayerPerspective, new UctEvaluator());

            try
            {
                await evaluationHandler.StartEvaluationAsync();
            }
            catch (OperationCanceledException)
            {
                // now I know the evaluation ended up with exception
            }

            evaluationState = BotEvaluationState.NotRunning;

            var bestMove = evaluationHandler.GetBestMove();

            return bestMove;
        }

        public override void UpdateMap()
        {
            throw new NotImplementedException();
        }

        public override void StopEvaluation()
        {
            if (evaluationState != BotEvaluationState.NotRunning)
            {
                evaluationState = BotEvaluationState.ShouldStop;
                evaluationHandler.Stop();
            }
        }
    }
}