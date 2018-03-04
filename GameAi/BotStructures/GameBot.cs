namespace GameAi.BotStructures
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using EvaluationStructures;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;
    using Interfaces;
    using InterFormatCommunication.GameRecording;

    /// <summary>
    /// Minimized version of <see cref="Game"/> from perspective of a given <see cref="Player"/>.
    /// </summary>
    internal abstract class GameBot : IOnlineBot<BotTurn>
    {
        protected readonly PlayerPerspective PlayerPerspective;

        public Difficulty Difficulty { get; }
        public bool IsFogOfWar { get; }

        public abstract bool CanStartEvaluation { get; }

        internal GameBot(PlayerPerspective playerPerspective, Difficulty difficulty, bool isFogOfWar)
        {
            this.PlayerPerspective = playerPerspective;
            this.Difficulty = difficulty;
            IsFogOfWar = isFogOfWar;
        }

        public abstract BotTurn GetCurrentBestMove();

        /// <summary>
        /// Asynchronously finds best move for the bot at given state.
        /// </summary>
        /// <returns></returns>
        public abstract Task<BotTurn> FindBestMoveAsync();

        public abstract void UpdateMap();

        public abstract void StopEvaluation();
    }
}