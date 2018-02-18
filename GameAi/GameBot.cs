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
        protected readonly PlayerPerspective PlayerPerspective;

        /// <summary>
        /// Dictionary containing information about mapping between original region ids and region ids this instance of bot uses.
        /// </summary>
        private readonly IdsMappingDictionary regionsIdsMappingDictionary;

        public Difficulty Difficulty { get; }
        public bool IsFogOfWar { get; }
        
        internal GameBot(PlayerPerspective playerPerspective, Difficulty difficulty, bool isFogOfWar, IdsMappingDictionary regionsIdsMappingDictionary)
        {
            this.PlayerPerspective = playerPerspective;
            this.Difficulty = difficulty;
            IsFogOfWar = isFogOfWar;
            this.regionsIdsMappingDictionary = regionsIdsMappingDictionary;
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

        protected int GetOriginalRegionId(int newRegionId)
        {
            if (!regionsIdsMappingDictionary.TryGetOriginalId(newRegionId, out int originalId))
            {
                throw new ArgumentException($"Region with new id {newRegionId} not found");
            }

            return originalId;
        }

        public void UpdateMap()
        {
            // TODO: break the evaluation

            // TODO: update the map
        }
    }
}