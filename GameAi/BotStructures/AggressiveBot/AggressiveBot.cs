namespace GameAi.BotStructures.AggressiveBot
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ActionGenerators;
    using Data;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Data.Restrictions;
    using Interfaces;
    using Interfaces.ActionsGenerators;
    using StructuresEvaluators;

    internal class AggressiveBot : GameBot
    {
        private readonly IGameBeginningActionsGenerator selectRegionActionsGenerator;
        private readonly AggressiveBotActionsGenerator actionsGenerator;
        private BotTurn generatedBestTurn;

        private readonly object botLock = new object();

        public AggressiveBot(PlayerPerspective playerPerspective,
            byte[] enemyPlayerId,
            Difficulty difficulty,
            bool isFogOfWar, Restrictions restrictions)
            : base(playerPerspective, enemyPlayerId, difficulty, isFogOfWar, restrictions)
        {
            selectRegionActionsGenerator = new SelectRegionActionsGenerator(
                new GameRegionMinEvaluator(new GameSuperRegionMinEvaluator(playerPerspective.MapMin)), 
                restrictions.GameBeginningRestrictions);
            actionsGenerator = new AggressiveBotActionsGenerator(
                new GameRegionMinEvaluator(new GameSuperRegionMinEvaluator(playerPerspective.MapMin)),
                playerPerspective.MapMin);
        }

        public override BotTurn GetCurrentBestMove()
        {
            throw new System.NotImplementedException();
        }

        public override async Task<BotTurn> FindBestMoveAsync()
        {
            lock (botLock)
            {
                if (EvaluationState != BotEvaluationState.NotRunning)
                {
                    throw new ArgumentException(
                        $"Cannot start evaluation if the current evaluation state is {EvaluationState}");
                }

                EvaluationState = BotEvaluationState.Running;
            }

            if (PlayerPerspective.MapMin.IsGameBeginning())
            {
                generatedBestTurn =
                    selectRegionActionsGenerator.Generate(PlayerPerspective)[0];
            }
            else
            {
                generatedBestTurn = actionsGenerator.Generate(PlayerPerspective)[0];
            }

            lock (botLock)
            {
                EvaluationState = BotEvaluationState.NotRunning;
            }

            return generatedBestTurn;
        }

        public override void UpdateMap()
        {
            throw new System.NotImplementedException();
        }

        public override void StopEvaluation()
        {
            lock (botLock)
            {
                if (EvaluationState != BotEvaluationState.NotRunning)
                {
                    EvaluationState = BotEvaluationState.ShouldStop;
                }
            }
        }
    }
}