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
            GameBot gameBot = new GameBot
            {
                regionsMin = game.Map.Regions.Select(x => new RegionMin(x, player)).ToArray(),
                superRegionsMin = game.Map.SuperRegions.Select(x => new SuperRegionMin(x, player)).ToArray()
            };
            // connect same nodes into one instance
            gameBot.ReconstructOriginalGraph();

            // get difficulty from Ai player
            if (player.GetType() == typeof(AiPlayer))
            {
                gameBot.difficulty = ((AiPlayer) player).Difficulty;
            }

            return gameBot;
        }

        /// <summary>
        ///     Purpose of this method is to reconnect references for future
        ///     graph updating to be easy.
        /// </summary>
        private void ReconstructOriginalGraph()
        {
            IList<RegionMin> regions = regionsMin;
            IList<SuperRegionMin> superRegions = superRegionsMin;
            // reconstruct original super regions
            {
                // SuperRegion = region.SuperRegion
                foreach (var region in regions)
                {
                    for (int j = 0; j < superRegions.Count; j++)
                    {
                        if (superRegions[j] == region.SuperRegion)
                        {
                            superRegions[j] = region.SuperRegion;
                        }
                    }
                }
            }
            // reconstruct super regions and regions
            {
                // superRegion.Region = region
                foreach (var region in regions)
                {
                    foreach (var superRegion in superRegions)
                    {
                        IList<RegionMin> superRegionRegions = superRegion.Regions;
                        for (int j = 0; j < superRegionRegions.Count; j++)
                        {
                            if (superRegionRegions[j] == region)
                            {
                                superRegionRegions[j] = region;
                            }
                        }
                    }
                }

                // region = superRegion.Region
                // should be fine
            }


            // reconstruct neighbour regions
            {
                for (int i = 0; i < regions.Count; i++)
                {
                    IList<RegionMin> neighbours = regions[i].NeighbourRegions;
                    for (int k = 0; k < neighbours.Count; k++)
                    {
                        for (int j = 0; j < regions.Count; j++)
                        {
                            if (regions[j] == neighbours[k])
                            {
                                neighbours[k] = regions[j];
                            }
                        }
                    }
                }
            }
        }

        public Round FindBestMove()
        {
            throw new NotImplementedException();
        }
    }
}