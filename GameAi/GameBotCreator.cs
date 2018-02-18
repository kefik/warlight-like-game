namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;

    public class GameBotCreator
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
            var superRegions = game.Map.SuperRegions.Select(x => dictionary.TryGetValue(player, out byte playerId) ? new SuperRegionMin(x, playerId) : new SuperRegionMin(x)).ToArray();

            // setup regions
            var regions = game.Map.Regions.Select(x =>
            {
                if (x.Owner == null)
                {
                    return new RegionMin(x, 0, player);
                }
                dictionary.TryGetValue(x.Owner, out byte encodedOwner);

                return new RegionMin(x, encodedOwner, player);
            }).ToArray();

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
            var map = new MapMinCreator().Create(regions, superRegions, out var regionsIdsMappingDictionary, out _);

            dictionary.TryGetValue(player, out byte playerEncoded);

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
                    gameBot = new MonteCarloTreeSearchBot(playerPerspective, difficulty, game.IsFogOfWar, regionsIdsMappingDictionary);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameBotType), gameBotType, null);
            }

            return gameBot;
        }

        public IBot<Turn> Create(GameBotType gameBotType, RegionMin[] regionsMin, SuperRegionMin[] superRegionsMin, Difficulty difficulty, byte playerEncoded, bool isFogOfWar)
        {
            // create minimized map
            // super regions mapping is not needed, because bot returns best move, which doesnt involve any super region iformation
            MapMin mapMin = new MapMinCreator().Create(regionsMin, superRegionsMin, out var regionIdsMappingDictionary, out _);

            PlayerPerspective playerPerspective = new PlayerPerspective(mapMin, playerEncoded);
            InitializeVisibility(playerPerspective, isFogOfWar);

            switch (gameBotType)
            {
                case GameBotType.MonteCarloTreeSearchBot:
                    return new MonteCarloTreeSearchBot(playerPerspective, difficulty, isFogOfWar, regionIdsMappingDictionary);
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameBotType), gameBotType, null);
            }
        }

        /// <summary>
        /// Initializes visibility of all regions based on <see cref="PlayerPerspective.PlayerEncoded"/>.
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
    }
}