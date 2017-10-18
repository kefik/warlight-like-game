namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;

    /// <summary>
    /// Minimized version of <see cref="Game"/> from perspective of a given <see cref="Player"/>.
    /// </summary>
    public class GameBot : IBot<Round>
    {
        private RegionMin[] regionsMin;
        private SuperRegionMin[] superRegionsMin;
        private Difficulty difficulty = Difficulty.Hard;

        /// <summary>
        /// Constructs minimized version of the game.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static IBot<Round> FromGame(Game game, Player player)
        {
            var regions = game.Map.Regions.Select(x => new RegionMin(x, player)).ToArray();
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

            var gameBot = new GameBot()
            {
                regionsMin = regions,
                superRegionsMin = superRegions
            };

            // get difficulty from Ai player
            if (player.GetType() == typeof(AiPlayer))
            {
                gameBot.difficulty = ((AiPlayer)player).Difficulty;
            }

            return gameBot;
        }

        public Round FindBestMove()
        {
            throw new NotImplementedException();
        }
    }
}