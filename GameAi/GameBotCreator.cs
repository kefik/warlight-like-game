namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BotStructures;
    using EvaluationStructures;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;
    using Interfaces;

    internal class GameBotCreator
    {
        public GameBot CreateFromGame(Game game, Player player, GameBotType gameBotType, out IdsMappingDictionary regionsIdsMappingDictionary)
        {
            if (!game.Players.Contains(player))
            {
                throw new ArgumentException("Incorrect player parameter.");
            }

            // setup super regions
            var superRegions = game.Map.SuperRegions
                .Select(x => x.Owner == null ? new SuperRegionMin(x.Id, x.Bonus) : new SuperRegionMin(x.Id, x.Bonus, (byte)x.Owner.Id)).ToArray();

            // setup regions
            var regions = game.Map.Regions
                .Select(x => x.Owner == null ? new RegionMin(x.Id, x.SuperRegion.Id, x.Army) : new RegionMin(x.Id, x.SuperRegion.Id, x.Army, (byte) x.Owner.Id)).ToArray();

            // setup neighbours to those regions
            for (int index = 0; index < regions.Length; index++)
            {
                var region = regions[index];
                // get original regions neighbours
                var originalNeighbours = game.Map.Regions.First(x => x.Id == region.Id).NeighbourRegions;

                region.NeighbourRegionsIds = originalNeighbours.Select(x => x.Id).ToArray();

                regions[index] = region;
            }

            for (int index = 0; index < superRegions.Length; index++)
            {
                var superRegion = superRegions[index];
                
                // get original SuperRegion regions
                var originalRegionsIds = game.Map.SuperRegions.First(x => x.Id == superRegion.Id).Regions.Select(x => x.Id);
                
                superRegion.RegionsIds = originalRegionsIds.ToArray();

                superRegions[index] = superRegion;
            }

            // create map
            var map = CreateMapForBot(regions, superRegions, out regionsIdsMappingDictionary, out _);

            byte playerEncoded = (byte) player.Id;

            var playerPerspective = new PlayerPerspective(map, playerEncoded);
            InitializeVisibility(playerPerspective, game.IsFogOfWar);

            Difficulty difficulty = Difficulty.Hard;
            if (player.GetType() == typeof(AiPlayer))
            {
                difficulty = ((AiPlayer)player).Difficulty;
            }

            GameBot gameBot;
            switch (gameBotType)
            {
                case GameBotType.MonteCarloTreeSearchBot:
                    gameBot = new MonteCarloTreeSearchBot(playerPerspective, difficulty, game.IsFogOfWar);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameBotType), gameBotType, null);
            }

            return gameBot;
        }

        public GameBot Create(GameBotType gameBotType, MapMin map, Difficulty difficulty, byte playerEncoded, bool isFogOfWar, out IdsMappingDictionary regionsIdsMappingDictionary)
        {
            // create minimized map
            // super regions mapping is not needed, because bot returns best move, which doesnt involve any super region iformation
            MapMin mapMin = CreateMapForBot(map, out regionsIdsMappingDictionary, out _);

            PlayerPerspective playerPerspective = new PlayerPerspective(mapMin, playerEncoded);
            InitializeVisibility(playerPerspective, isFogOfWar);

            switch (gameBotType)
            {
                case GameBotType.MonteCarloTreeSearchBot:
                    return new MonteCarloTreeSearchBot(playerPerspective, difficulty, isFogOfWar);
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameBotType), gameBotType, null);
            }
        }

        /// <summary>
        /// Initializes visibility of all regions based on <see cref="PlayerPerspective.PlayerId"/>.
        /// </summary>
        /// <param name="playerPerspective"></param>
        /// <param name="isFogOfWar"></param>
        private void InitializeVisibility(PlayerPerspective playerPerspective, bool isFogOfWar)
        {
            if (isFogOfWar)
            {
                for (int index = 0; index < playerPerspective.MapMin.RegionsMin.Length; index++)
                {
                    playerPerspective.MapMin.RegionsMin[index].IsVisible = true;
                }
            }
            else
            {
                for (int index = 0; index < playerPerspective.MapMin.RegionsMin.Length; index++)
                {
                    var regionMin = playerPerspective.MapMin.RegionsMin[index];
                    if (playerPerspective.IsRegionMine(regionMin)
                        || playerPerspective.IsNeighbourToMyRegion(regionMin))
                    {
                        playerPerspective.MapMin.RegionsMin[index].IsVisible = true;
                    }
                }
            }
        }

        /// <summary>
        /// Creates <see cref="MapMin"/> instance that bot can work with.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="regionIdsMappingDictionary"></param>
        /// <param name="superRegionsIdsMappingDictionary"></param>
        /// <returns></returns>
        private MapMin CreateMapForBot(MapMin map, out IdsMappingDictionary regionIdsMappingDictionary,
            out IdsMappingDictionary superRegionsIdsMappingDictionary)
        {
            return CreateMapForBot(map.RegionsMin, map.SuperRegionsMin, out regionIdsMappingDictionary,
                out superRegionsIdsMappingDictionary);
        }
        /// <summary>
        /// Creates MapMin instance.
        /// </summary>
        /// <param name="regionsMin"></param>
        /// <param name="superRegionsMin"></param>
        /// <param name="regionIdsMappingDictionary"></param>
        /// <param name="superRegionsIdsMappingDictionary"></param>
        /// <returns></returns>
        private MapMin CreateMapForBot(RegionMin[] regionsMin, SuperRegionMin[] superRegionsMin, out IdsMappingDictionary regionIdsMappingDictionary, out IdsMappingDictionary superRegionsIdsMappingDictionary)
        {
            regionIdsMappingDictionary = new IdsMappingDictionary();
            superRegionsIdsMappingDictionary = new IdsMappingDictionary();

            // we dont want to change original structures => deep copy
            var tempMapMin = new MapMin(regionsMin, superRegionsMin).DeepCopy();
            regionsMin = tempMapMin.RegionsMin.OrderBy(x => x.Id).ToArray();
            superRegionsMin = tempMapMin.SuperRegionsMin.OrderBy(x => x.Id).ToArray();

            // so far we expect Ids to be ordered and unique
            // now we have to shuffle it to minimal values => e.g. 2, 5, 7 => 0, 1, 2

            for (int i = 0; i < regionsMin.Length; i++)
            {
                ref RegionMin region = ref regionsMin[i];

                int mappedId = regionIdsMappingDictionary.GetMappedIdOrInsert(regionsMin[i].Id);

                RemapId(regionsMin, superRegionsMin, ref region, mappedId);

                region.Id = mappedId;

            }

            // do the same with super regions
            for (int i = 0; i < superRegionsMin.Length; i++)
            {
                ref SuperRegionMin superRegion = ref superRegionsMin[i];

                int mappedId = superRegionsIdsMappingDictionary.GetMappedIdOrInsert(superRegionsMin[i].Id);

                RemapId(regionsMin, ref superRegion, mappedId);

                superRegion.Id = mappedId;
            }

            return new MapMin(regionsMin, superRegionsMin);
        }

        private void RemapId(RegionMin[] regionsMin, SuperRegionMin[] superRegionsMin, ref RegionMin regionMin, int newRegionId)
        {
            var neighboursIds = regionMin.NeighbourRegionsIds;

            var neighbours = regionsMin.Where(x => neighboursIds.Contains(x.Id));

            // remap neighbours
            // for each neighbour change ID of regionMin to newId
            foreach (var neighbour in neighbours)
            {
                var neighbourNeighbours = neighbour.NeighbourRegionsIds;

                for (int i = 0; i < neighbourNeighbours.Length; i++)
                {
                    if (neighbourNeighbours[i] == regionMin.Id)
                    {
                        neighbourNeighbours[i] = newRegionId;
                    }
                }
            }

            // remap super regions regions
            int superRegionId = regionMin.SuperRegionId;

            var superRegion = superRegionsMin.First(x => x.Id == superRegionId);
            int[] superRegionRegionsIds = superRegion.RegionsIds;

            for (int i = 0; i < superRegionRegionsIds.Length; i++)
            {
                if (superRegionRegionsIds[i] == regionMin.Id)
                {
                    superRegionRegionsIds[i] = newRegionId;
                }
            }
        }

        private void RemapId(RegionMin[] regionsMin, ref SuperRegionMin currentSuperRegionMin, int newSuperRegionId)
        {
            int oldSuperRegionId = currentSuperRegionMin.Id;

            // remap super regions regions
            var superRegionsRegions = regionsMin.Where(x => x.SuperRegionId == oldSuperRegionId);

            foreach (RegionMin superRegionsRegion in superRegionsRegions)
            {
                RegionMin regionsRegion = superRegionsRegion;

                regionsRegion.SuperRegionId = newSuperRegionId;
            }
        }
    }
}