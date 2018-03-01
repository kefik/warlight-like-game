﻿namespace GameAi.BotStructures
{
    using System;
    using System.Threading.Tasks;
    using EvaluationStructures;
    using GameObjectsLib;
    using GameObjectsLib.GameRecording;
    using GameRecording;

    /// <summary>
    /// Bot using Monte-Carlo tree search algorithm.
    /// </summary>
    internal class MonteCarloTreeSearchBot : GameBot
    {
        private BotEvaluationState evaluationState;

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

            // TODO: add evaluation

            evaluationState = BotEvaluationState.NotRunning;
            throw new NotSupportedException();
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
            }
        }
    }
}