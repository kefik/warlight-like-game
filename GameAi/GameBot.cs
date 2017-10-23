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
        protected Difficulty Difficulty { get; private set; } = Difficulty.Hard;
        protected internal bool IsFogOfWar { get; private set; }

        protected PlayerPerspective PlayerPerspective;

        /// <summary>
        /// Constructs minimized version of the game.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="player"></param>
        /// <param name="gameBotType"></param>
        /// <returns></returns>
        public static GameBot FromGame(Game game, Player player, GameBotType gameBotType)
        {
            if (!game.Players.Contains(player))
            {
                throw new ArgumentException("Incorrect player parameter.");
            }

            var dictionary = new Dictionary<Player, byte>();

            for (byte i = 0; i < game.Players.Count; i++)
            {
                dictionary.Add(game.Players[i], (byte)(i + 1));
            }

            var regions = game.Map.Regions.Select(x =>
            {
                if (x.Owner == null)
                {
                    return new RegionMin(x, 0, player, game.IsFogOfWar);
                }
                dictionary.TryGetValue(x.Owner, out byte encodedOwner);
                
                return new RegionMin(x, encodedOwner, player, game.IsFogOfWar);
            }).ToArray();
            foreach (var region in regions)
            {
                List<RegionMin> neighbours = new List<RegionMin>();
                // get original regions neighbours
                var originalNeighbours = game.Map.Regions.First(x => x.Id == region.Id).NeighbourRegions;

                // for-each neighbour find his equivalent in regions and add it to neighbour
                foreach (var originalNeighbour in originalNeighbours)
                {
                    var realNeighbour = regions.First(x => x.Id == originalNeighbour.Id);
                    neighbours.Add(realNeighbour);
                }

                // copy it to the array
                region.NeighbourRegions = neighbours.ToArray();
            }

            var superRegions = game.Map.SuperRegions.Select(x => new SuperRegionMin(x, player)).ToArray();
            foreach (var superRegion in superRegions)
            {
                List<RegionMin> containedRegions = new List<RegionMin>();
                // get original SuperRegion regions
                var originalRegions = game.Map.SuperRegions.First(x => x.Id == superRegion.Id).Regions;

                foreach (var originalRegion in originalRegions)
                {
                    var realRegion = regions.First(x => x.Id == originalRegion.Id);
                    containedRegions.Add(realRegion);
                }

                superRegion.Regions = containedRegions.ToArray();
            }

            GameBot gameBot;
            switch (gameBotType)
            {
                case GameBotType.MonteCarloTreeSearchBot:
                    gameBot = new MonteCarloTreeSearchBot();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameBotType), gameBotType, null);
            }
            {
                dictionary.TryGetValue(player, out byte playerEncoded);
                gameBot.PlayerPerspective.PlayerEncoded = playerEncoded;
                gameBot.PlayerPerspective.RegionsMin = regions;
                gameBot.PlayerPerspective.SuperRegionsMin = superRegions;
                gameBot.IsFogOfWar = game.IsFogOfWar;
            }

            // get difficulty from Ai player
            if (player.GetType() == typeof(AiPlayer))
            {
                gameBot.Difficulty = ((AiPlayer)player).Difficulty;
            }

            return gameBot;
        }

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