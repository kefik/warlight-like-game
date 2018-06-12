namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Collections;
    using Common.Extensions;
    using Data;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Data.Restrictions;
    using Interfaces;

    /// <summary>
    /// Bot using Monte-Carlo tree search algorithm.
    /// </summary>
    internal class MonteCarloTreeSearchBot : GameBot
    {
        private readonly MCTSEvaluationHandler evaluationHandler;
        private object botLock = new object();

        public MonteCarloTreeSearchBot(
            PlayerPerspective playerPerspective,
            byte enemyPlayerId,
            Difficulty difficulty, bool isFogOfWar,
            Restrictions restrictions)
            : base(playerPerspective, new byte[] { enemyPlayerId }, difficulty, isFogOfWar, restrictions)
        {
            evaluationHandler = new MCTSEvaluationHandler(PlayerPerspective,
                enemyPlayerId,
                restrictions);
        }

        public override BotTurn GetCurrentBestMove()
        {
            if (evaluationHandler == null)
            {
                throw new ArgumentException("Bot hasn't been started yet.");
            }
            return evaluationHandler.GetBestMove();
        }

        public override void UseFixedDeploy(IEnumerable<BotDeployment> deploymentsToUse)
        {
            evaluationHandler.UseFixedDeploy(deploymentsToUse);
        }

        public override async Task<BotTurn> FindBestMoveAsync()
        {
            if (EvaluationState != BotEvaluationState.NotRunning)
            {
                throw new ArgumentException($"Cannot start evaluation if the current evaluation state is {EvaluationState}");
            }

            try
            {
                var evaluationTask = evaluationHandler.StartEvaluationAsync();

                // must be called here to avoid race condition (stop before start)
                EvaluationState = BotEvaluationState.Running;

                await evaluationTask;
            }
            catch (OperationCanceledException)
            {
                // now I know the evaluation ended up with exception
            }

            EvaluationState = BotEvaluationState.NotRunning;

            var bestMove = evaluationHandler.GetBestMove();

            return bestMove;
        }

        public override void UpdateMap()
        {
            throw new NotImplementedException();
        }

        public override void StopEvaluation()
        {
            if (EvaluationState != BotEvaluationState.NotRunning)
            {
                EvaluationState = BotEvaluationState.ShouldStop;
                evaluationHandler.Stop();
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}