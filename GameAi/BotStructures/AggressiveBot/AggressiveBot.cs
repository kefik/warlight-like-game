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
    using Interfaces.Evaluators.StructureEvaluators;
    using StructuresEvaluators;

    internal class AggressiveBot : GameBot
    {
        private readonly IGameBeginningActionsGenerator gameBeginningActionsGenerator;
        private readonly IGameActionsGenerator gameActionsGenerator;
        private BotTurn generatedBestTurn;

        private readonly object botLock = new object();

        public AggressiveBot(PlayerPerspective playerPerspective,
            byte[] enemyPlayerId,
            Difficulty difficulty,
            bool isFogOfWar, Restrictions restrictions)
            : base(playerPerspective, enemyPlayerId, difficulty, isFogOfWar, restrictions)
        {
            ISuperRegionMinEvaluator superRegionMinEvaluator = new GameSuperRegionMinEvaluator(playerPerspective.MapMin,
                bonusCoefficient: 15, foreignNeighboursCoefficient: 4, superRegionsRegionsCountCoefficient: 5);
            IRegionMinEvaluator regionMinEvaluator = new GameRegionMinEvaluator(superRegionMinEvaluator,
                bonusCoefficient: 15, superRegionCoefficient: 3);

            ISuperRegionMinEvaluator gameBeginningSuperRegionMinEvaluator = new GameBeginningSuperRegionMinEvaluator(playerPerspective.MapMin,
                superRegionsRegionsCountCoefficient: 5, foreignNeighboursCoefficient: 4, bonusCoefficient: 1);
            IRegionMinEvaluator gameBeginningRegionMinEvaluator = new GameBeginningRegionMinEvaluator(gameBeginningSuperRegionMinEvaluator,
                playerPerspective.MapMin, clusterCoefficient: 50, superRegionCoefficient: 3);

            gameBeginningActionsGenerator = new SelectRegionActionsGenerator(
                gameBeginningRegionMinEvaluator, 
                restrictions.GameBeginningRestrictions);
            gameActionsGenerator = new AggressiveBotActionsGenerator(
                regionMinEvaluator,
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
                    gameBeginningActionsGenerator.Generate(PlayerPerspective)[0];
            }
            else
            {
                generatedBestTurn = gameActionsGenerator.Generate(PlayerPerspective)[0];
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