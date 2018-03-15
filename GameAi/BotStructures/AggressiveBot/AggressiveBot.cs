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
        private readonly SelectRegionActionGenerator selectRegionActionGenerator;
        private readonly AggressiveBotActionGenerator actionGenerator;
        private BotTurn generatedBestTurn;

        private readonly object botLock = new object();

        public AggressiveBot(PlayerPerspective playerPerspective, Difficulty difficulty,
            bool isFogOfWar, Restrictions restrictions)
            : base(playerPerspective, difficulty, isFogOfWar, restrictions)
        {
            var gameBeginningRestriction = restrictions
                .GameBeginningRestrictions.First(x => x.PlayerId == playerPerspective.PlayerId);
            selectRegionActionGenerator = new SelectRegionActionGenerator(
                gameBeginningRestriction.RegionsPlayerCanChooseCount, 
                gameBeginningRestriction.PlayerId,
                gameBeginningRestriction.RestrictedRegions);
            actionGenerator = new AggressiveBotActionGenerator();
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
                    selectRegionActionGenerator.Generate(PlayerPerspective);
            }
            else
            {
                generatedBestTurn = actionGenerator.Generate(PlayerPerspective);
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