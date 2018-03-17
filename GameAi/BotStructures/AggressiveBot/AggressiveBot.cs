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

    internal class AggressiveBot : GameBot
    {
        private readonly RandomSelectRegionActionsGenerator randomSelectRegionActionsGenerator;
        private readonly AggressiveBotActionsGenerator actionsGenerator;
        private BotTurn generatedBestTurn;

        private readonly object botLock = new object();

        public AggressiveBot(PlayerPerspective playerPerspective,
            byte[] playersIds,
            Difficulty difficulty,
            bool isFogOfWar, Restrictions restrictions)
            : base(playerPerspective, playersIds, difficulty, isFogOfWar, restrictions)
        {
            var gameBeginningRestriction = restrictions
                .GameBeginningRestrictions.First(x => x.PlayerId == playerPerspective.PlayerId);
            randomSelectRegionActionsGenerator = new RandomSelectRegionActionsGenerator(
                gameBeginningRestriction.RegionsPlayerCanChooseCount, 
                gameBeginningRestriction.PlayerId,
                gameBeginningRestriction.RestrictedRegions);
            actionsGenerator = new AggressiveBotActionsGenerator();
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
                    randomSelectRegionActionsGenerator.Generate(PlayerPerspective)[0];
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