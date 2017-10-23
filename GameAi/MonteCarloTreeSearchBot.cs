namespace GameAi
{
    using System;
    using System.Threading.Tasks;
    using GameObjectsLib;

    /// <summary>
    /// Bot using Monte-Carlo tree search algorithm.
    /// </summary>
    public class MonteCarloTreeSearchBot : GameBot
    {
        public override Round FindBestMove()
        {
            var playerPerspective = PlayerPerspective.ShallowCopy();
            throw new NotImplementedException();
        }

        public override Task<Round> FindBestMoveAsync()
        {
            throw new NotSupportedException();
        }

        private Round FindBestMove(PlayerPerspective playerPerspective)
        {
            throw new NotImplementedException();
        }
    }
}