namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;

    /// <summary>
    /// Minimized version of <see cref="Game"/> from perspective of a given <see cref="Player"/>.
    /// </summary>
    public abstract class GameBot : IBot<Round>
    {
        internal GameBot(PlayerPerspective playerPerspective, Difficulty difficulty)
        {
            this.PlayerPerspective = playerPerspective;
            this.Difficulty = difficulty;
        }

        protected internal Difficulty Difficulty { get; }
        protected internal bool IsFogOfWar { get; set; }

        protected PlayerPerspective PlayerPerspective { get; }
        
        /// <summary>
        /// Finds and returns best move for given bot state.
        /// </summary>
        /// <returns></returns>
        public abstract Round FindBestMove();

        /// <summary>
        /// Asynchronously finds best move for the bot at given state.
        /// </summary>
        /// <returns></returns>
        public abstract Task<Round> FindBestMoveAsync();
    }
}