namespace GameAi.BotStructures
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EvaluationStructures;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;
    using GameRecording;
    using Interfaces;

    /// <summary>
    /// Minimized version of <see cref="Game"/> from perspective of a given <see cref="Player"/>.
    /// </summary>
    internal abstract class GameBot : IOnlineBot<BotTurn>
    {
        protected readonly PlayerPerspective PlayerPerspective;

        public Difficulty Difficulty { get; }
        public bool IsFogOfWar { get; }

        public BotTurn CurrentBestMove { get; protected set; }

        internal GameBot(PlayerPerspective playerPerspective, Difficulty difficulty, bool isFogOfWar)
        {
            this.PlayerPerspective = playerPerspective;
            this.Difficulty = difficulty;
            IsFogOfWar = isFogOfWar;
        }

        /// <summary>
        /// Finds and returns best move for given bot state.
        /// </summary>
        /// <returns></returns>
        public abstract BotTurn FindBestMove();

        /// <summary>
        /// Asynchronously finds best move for the bot at given state.
        /// </summary>
        /// <returns></returns>
        public abstract Task<BotTurn> FindBestMoveAsync();
        
        public void UpdateMap()
        {
            // TODO: break the evaluation

            // TODO: update the map
        }

        public void StopEvaluation()
        {
            throw new NotImplementedException();
        }
    }
}