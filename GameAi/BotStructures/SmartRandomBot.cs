namespace GameAi.BotStructures
{
    using System;
    using System.Threading.Tasks;
    using ActionGenerators;
    using Data;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Data.Restrictions;
    using Interfaces.Evaluators;
    using Interfaces.Evaluators.StructureEvaluators;
    using StructuresEvaluators;

    internal class SmartRandomBot : GameBot
    {
        private readonly ISuperRegionMinEvaluator
            gameBeginningSuperRegionMinEvaluator;

        private readonly IRegionMinEvaluator gameBeginningRegionMinEvaluator;

        private readonly ISuperRegionMinEvaluator gameSuperRegionMinEvaluator;

        private readonly IRegionMinEvaluator gameRegionMinEvaluator;

        private readonly IRoundEvaluator roundEvaluator;

        private readonly MCTSBotActionsGenerator gameActionsGenerator;

        private readonly SelectRegionActionsGenerator selectRegionActionsGenerator;

        private Random random;

        public SmartRandomBot(PlayerPerspective playerPerspective,
            byte enemyPlayerId,
            Difficulty difficulty,
            bool isFogOfWar,
            Restrictions restrictions)
            : base(playerPerspective, new byte[] { enemyPlayerId }, difficulty, isFogOfWar, restrictions)
        {
            DistanceMatrix distanceMatrix =
                new DistanceMatrix(playerPerspective.MapMin
                    .RegionsMin);
            gameBeginningSuperRegionMinEvaluator =
                    new GameBeginningSuperRegionMinEvaluator(
                        playerPerspective.MapMin,
                        superRegionsRegionsCountCoefficient: 3,
                        foreignNeighboursCoefficient: 5,
                        bonusCoefficient: 3);
            gameBeginningRegionMinEvaluator =
                new GameBeginningRegionMinEvaluator(
                    gameBeginningSuperRegionMinEvaluator,
                    distanceMatrix, clusterCoefficient: 50,
                    superRegionCoefficient: 3);

            gameSuperRegionMinEvaluator =
                new GameSuperRegionMinEvaluator(
                    playerPerspective.MapMin, bonusCoefficient: 3,
                    conqueredCoefficient: 30,
                    foreignNeighboursCoefficient: 3,
                    superRegionsRegionsCountCoefficient: 5);
            gameRegionMinEvaluator =
                new GameRegionMinEvaluator(
                    gameSuperRegionMinEvaluator,
                    superRegionCoefficient: 20);

            roundEvaluator = new RoundEvaluator();
            IPlayerPerspectiveEvaluator gamePlayerPerspectiveEvaluator
                = new PlayerPerspectiveEvaluator(
                    gameRegionMinEvaluator, armyCoefficient: 10);

            gameActionsGenerator =
                new MCTSBotActionsGenerator(gameRegionMinEvaluator,
                    gameSuperRegionMinEvaluator,
                    new byte[] { enemyPlayerId, playerPerspective.PlayerId },
                    playerPerspective.MapMin);

            selectRegionActionsGenerator
                = new SelectRegionActionsGenerator(
                    gameBeginningRegionMinEvaluator,
                    restrictions.GameBeginningRestrictions);

            this.random = new Random();
        }

        public override BotTurn GetCurrentBestMove()
        {
            throw new System.NotImplementedException();
        }

        public override async Task<BotTurn> FindBestMoveAsync()
        {
            BotTurn turn;
            if (PlayerPerspective.MapMin.IsGameBeginning())
            {
                var turns =
                    selectRegionActionsGenerator.Generate(
                        PlayerPerspective);

                turn = turns[random.Next(turns.Count)];
            }
            else
            {
                var turns = gameActionsGenerator.Generate(PlayerPerspective);

                turn = turns[random.Next(turns.Count)];
            }

            return turn;
        }

        public override void UpdateMap()
        {
            throw new System.NotImplementedException();
        }

        public override void StopEvaluation()
        {
            // has almost instant evaluation anyway, no reason to break
        }
    }
}