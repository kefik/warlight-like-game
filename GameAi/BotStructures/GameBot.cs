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
    using Interfaces;

    /// <summary>
    /// Minimized version of <see cref="Game"/> from perspective of a given <see cref="Player"/>.
    /// </summary>
    internal abstract class GameBot : IOnlineBot<Turn>
    {
        protected readonly PlayerPerspective PlayerPerspective;

        public Difficulty Difficulty { get; }
        public bool IsFogOfWar { get; }

        public Turn CurrentBestMove { get; protected set; }

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
        public abstract Turn FindBestMove();

        /// <summary>
        /// Asynchronously finds best move for the bot at given state.
        /// </summary>
        /// <returns></returns>
        public abstract Task<Turn> FindBestMoveAsync();
        
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