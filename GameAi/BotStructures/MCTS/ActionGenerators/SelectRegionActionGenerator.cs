namespace GameAi.BotStructures.MCTS.ActionGenerators
{
    using System.Collections.Generic;
    using EvaluationStructures;
    using GameRecording;
    using Interfaces;

    internal class SelectRegionActionGenerator : IGameActionGenerator<BotGameBeginningTurn, PlayerPerspective>
    {
        private readonly IEnumerable<int> regionsRestrictions;

        public SelectRegionActionGenerator(IEnumerable<int> regionsRestrictions = null)
        {
            this.regionsRestrictions = regionsRestrictions;
        }
        public BotGameBeginningTurn Generate(PlayerPerspective currentGameState)
        {
            throw new System.NotImplementedException();
        }
    }
}