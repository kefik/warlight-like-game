namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;

    /// <summary>
    /// Minimized version of <see cref="Game"/> from perspective of a given <see cref="Player"/>.
    /// </summary>
    public abstract class GameBot : IBot<Turn>
    {
        public readonly PlayerPerspective PlayerPerspective;

        public Difficulty Difficulty { get; }
        public bool IsFogOfWar { get; }
        
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
    }
}