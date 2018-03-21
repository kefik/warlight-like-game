namespace GameAi.BotStructures.ActionGenerators
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Interfaces.ActionsGenerators;
    using Interfaces.Evaluators.StructureEvaluators;

    public class MCTSBotActionsGenerator : GameActionsGenerator, IGameActionsGenerator
    {
        private readonly IRegionMinEvaluator regionMinEvaluator;
        private readonly byte enemyPlayerId;

        public MCTSBotActionsGenerator(IRegionMinEvaluator regionMinEvaluator,
            MapMin mapMin,
            byte enemyPlayerId) : base(new DistanceMatrix(mapMin.RegionsMin), regionMinEvaluator)
        {
            this.regionMinEvaluator = regionMinEvaluator;
            this.enemyPlayerId = enemyPlayerId;
        }

        public IReadOnlyList<BotGameTurn> Generate(PlayerPerspective currentGameState)
        {
            // attack

            throw new System.NotImplementedException();
        }
    }
}