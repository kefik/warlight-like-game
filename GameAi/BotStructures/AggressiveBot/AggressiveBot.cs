//#define EVALUATORS_DEBUG

#if !DEBUG
#undef EVALUATORS_DEBUG
#endif

namespace GameAi.BotStructures.AggressiveBot
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using ActionGenerators;
    using Data;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Data.Restrictions;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.StructureEvaluators;
    using StructuresEvaluators;

    internal class AggressiveBot : GameBot
    {
        private readonly IGameBeginningActionsGenerator
            gameBeginningActionsGenerator;

        private readonly AggressiveBotActionsGenerator
            gameActionsGenerator;

        private BotTurn generatedBestTurn;

        private readonly object botLock = new object();

        public AggressiveBot(PlayerPerspective playerPerspective,
            byte[] enemyPlayerId, Difficulty difficulty,
            bool isFogOfWar, Restrictions restrictions) : base(
            playerPerspective, enemyPlayerId, difficulty, isFogOfWar,
            restrictions)
        {
            ISuperRegionMinEvaluator superRegionMinEvaluator =
                new GameSuperRegionMinEvaluator(
                    playerPerspective.MapMin, bonusCoefficient: 3,
                    conqueredCoefficient: 5,
                    foreignNeighboursCoefficient: 3,
                    superRegionsRegionsCountCoefficient: 5);
            IRegionMinEvaluator regionMinEvaluator =
                new GameRegionMinEvaluator(superRegionMinEvaluator,
                    superRegionCoefficient: 20);

            ISuperRegionMinEvaluator
                gameBeginningSuperRegionMinEvaluator =
                    new GameBeginningSuperRegionMinEvaluator(
                        playerPerspective.MapMin,
                        superRegionsRegionsCountCoefficient: 6,
                        foreignNeighboursCoefficient: 5,
                        bonusCoefficient: 3);
            IRegionMinEvaluator gameBeginningRegionMinEvaluator =
                new GameBeginningRegionMinEvaluator(
                    gameBeginningSuperRegionMinEvaluator,
                    playerPerspective.MapMin, clusterCoefficient: 50,
                    superRegionCoefficient: 15);

            gameBeginningActionsGenerator =
                new SelectRegionActionsGenerator(
                    gameBeginningRegionMinEvaluator,
                    restrictions.GameBeginningRestrictions);
            gameActionsGenerator =
                new AggressiveBotActionsGenerator(regionMinEvaluator,
                    playerPerspective.MapMin);


#if EVALUATORS_DEBUG
            Debug.WriteLine(
                $"EVALUATION STARTED (BOT {PlayerPerspective.PlayerId})");
            Debug.WriteLine("------------");

            if (PlayerPerspective.MapMin.IsGameBeginning())
            {
                Debug.WriteLine("Game beginning phase");
                Debug.WriteLine("SuperRegions:");
                foreach (SuperRegionMin superRegion in
                    PlayerPerspective.MapMin.SuperRegionsMin.OrderBy(
                        x => x.Name))
                {
                    Debug.WriteLine(
                        $"Name: {superRegion.Name}, Value: " +
                        $"{gameBeginningSuperRegionMinEvaluator.GetValue(PlayerPerspective, superRegion):F1}");
                }
                Debug.WriteLine("");

                Debug.WriteLine($"Regions:");
                foreach (RegionMin region in PlayerPerspective.MapMin
                    .RegionsMin.OrderBy(x => x.Name))
                {
                    Debug.WriteLine($"Name: {region.Name}, Value: " +
                                    $"{gameBeginningRegionMinEvaluator.GetValue(PlayerPerspective, region):F1}");
                }
                Debug.WriteLine("");
            }
            else
            {
                Debug.WriteLine("SuperRegions:");
                foreach (SuperRegionMin superRegion in
                    PlayerPerspective.MapMin.SuperRegionsMin.OrderBy(
                        x => x.Name))
                {
                    Debug.WriteLine(
                        $"Name: {superRegion.Name}, Value: " +
                        $"{superRegionMinEvaluator.GetValue(PlayerPerspective, superRegion):F1}");
                }
                Debug.WriteLine("");

                Debug.WriteLine($"Regions:");
                foreach (RegionMin region in PlayerPerspective.MapMin
                    .RegionsMin.OrderBy(x => x.Name))
                {
                    Debug.WriteLine($"Name: {region.Name}, Value: " +
                                    $"{regionMinEvaluator.GetValue(PlayerPerspective, region):F1}");
                }
                Debug.WriteLine("");
            }
#endif
        }

        public override BotTurn GetCurrentBestMove()
        {
            throw new NotImplementedException();
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
                    gameBeginningActionsGenerator.Generate(
                        PlayerPerspective)[index: 0];
            }
            else
            {
                generatedBestTurn =
                    gameActionsGenerator.Generate(PlayerPerspective)[
                        index: 0];
            }

            lock (botLock)
            {
                EvaluationState = BotEvaluationState.NotRunning;
            }

            return generatedBestTurn;
        }

        public override void UpdateMap()
        {
            throw new NotImplementedException();
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