﻿namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;

    public class GameBotFactory
    {
        public IBot<Turn> CreateFromGame(Game game, Player player, GameBotType gameBotType)
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

            // setup super regions
            var superRegions = game.Map.SuperRegions.Select(x => new SuperRegionMin(x, player)).ToArray();

            // setup regions
            var regions = game.Map.Regions.Select(x =>
            {
                if (x.Owner == null)
                {
                    return new RegionMin(x, 0, player, game.IsFogOfWar);
                }
                dictionary.TryGetValue(x.Owner, out byte encodedOwner);

                return new RegionMin(x, encodedOwner, player, game.IsFogOfWar);
            }).ToArray();

            // setup neighbours to those regions
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

            var map = new MapMin(regions, superRegions);

            map.ReconstructGraph();

            dictionary.TryGetValue(player, out byte playerEncoded);
            var playerPerspective = new PlayerPerspective(map, playerEncoded);

            Difficulty difficulty = Difficulty.Hard;
            if (player.GetType() == typeof(AiPlayer))
            {
                difficulty = ((AiPlayer)player).Difficulty;
            }

            GameBot gameBot;
            switch (gameBotType)
            {
                case GameBotType.MonteCarloTreeSearchBot:
                    gameBot = new MonteCarloTreeSearchBot(playerPerspective, difficulty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameBotType), gameBotType, null);
            }

            return gameBot;
        }

        public IBot<Turn> Create(GameBotType gameBotType, MapMin mapMin, Difficulty difficulty, byte playerEncoded)
        {
            switch (gameBotType)
            {
                case GameBotType.MonteCarloTreeSearchBot:
                    return new MonteCarloTreeSearchBot(new PlayerPerspective(mapMin, playerEncoded), difficulty);
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameBotType), gameBotType, null);
            }
        }
    }
}