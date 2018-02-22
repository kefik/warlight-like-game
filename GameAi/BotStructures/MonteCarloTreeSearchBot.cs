namespace GameAi.BotStructures
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
        public MonteCarloTreeSearchBot(PlayerPerspective playerPerspective, Difficulty difficulty, bool isFogOfWar)
            : base(playerPerspective, difficulty, isFogOfWar)
        {
        }

        public override BotTurn FindBestMove()
        {
            var playerPerspective = PlayerPerspective.ShallowCopy();
            throw new NotImplementedException();
        }

        public override Task<BotTurn> FindBestMoveAsync()
        {
            throw new NotSupportedException();
        }

        private Round FindBestMove(PlayerPerspective playerPerspective)
        {
            throw new NotImplementedException();
        }
    }
}