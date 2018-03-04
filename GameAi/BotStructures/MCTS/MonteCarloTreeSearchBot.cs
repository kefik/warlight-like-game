namespace GameAi.BotStructures.MCTS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ActionGenerators;
    using Common.Collections;
    using Common.Extensions;
    using EvaluationStructures;
    using GameObjectsLib;
    using Interfaces;
    using InterFormatCommunication.GameRecording;
    using InterFormatCommunication.Restrictions;

    /// <summary>
    /// Bot using Monte-Carlo tree search algorithm.
    /// </summary>
    internal class MonteCarloTreeSearchBot : GameBot
    {
        private BotEvaluationState evaluationState;
        private readonly MCTSEvaluationHandler evaluationHandler;

        public override bool CanStartEvaluation
        {
            get { return evaluationState == BotEvaluationState.NotRunning; }
        }

        public MonteCarloTreeSearchBot(PlayerPerspective playerPerspective, Difficulty difficulty, bool isFogOfWar,
            Restrictions restrictions)
            : base(playerPerspective, difficulty, isFogOfWar, restrictions)
        {
            // there must be game beginning restriction specifying initial conditions
            if (Restrictions?.GameBeginningRestrictions == null)
            {
                throw new ArgumentNullException();
            }

            var gameBeginningRestriction =
                Restrictions.GameBeginningRestrictions
                    .First(x => x.PlayerId == PlayerPerspective.PlayerId);

            evaluationHandler = new MCTSEvaluationHandler(PlayerPerspective,
                new UctEvaluator(),
                new SelectRegionActionGenerator(
                    gameBeginningRestriction.RegionsPlayerCanChooseCount,
                    gameBeginningRestriction.PlayerId,
                    gameBeginningRestriction.RestrictedRegions),
                new AggressiveBotActionGenerator());
        }

        public override BotTurn GetCurrentBestMove()
        {
            if (evaluationHandler == null)
            {
                throw new ArgumentException("Bot hasn't been started yet.");
            }
            return evaluationHandler.GetBestMove();
        }

        public override async Task<BotTurn> FindBestMoveAsync()
        {
            if (evaluationState != BotEvaluationState.NotRunning)
            {
                throw new ArgumentException($"Cannot start evaluation if the current evaluation state is {evaluationState}");
            }
            
            try
            {
                var evaluationTask = evaluationHandler.StartEvaluationAsync();

                // must be called here to avoid race condition (stop before start)
                evaluationState = BotEvaluationState.Running;

                await evaluationTask;
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