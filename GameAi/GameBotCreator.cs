namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BotStructures;
    using BotStructures.AggressiveBot;
    using BotStructures.MCTS;
    using Data;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Data.Restrictions;
    using FormatConverters;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameRecording;
    using GameObjectsLib.Players;
    using Interfaces;

    internal class GameBotCreator
    {
        public IOnlineBot<BotTurn> Create(GameBotType gameBotType,
            MapMin map,
            Difficulty difficulty,
            byte playerEncoded,bool isFogOfWar,
            out IdsMappingDictionary regionsIdsMappingDictionary,
            Restrictions restrictions)
        {
            // create minimized map
            // super regions mapping is not needed, because bot returns best move, which doesnt involve any super region iformation
            MapMin mapMin = CreateMapForBot(map, out regionsIdsMappingDictionary, out _);

            PlayerPerspective playerPerspective = new PlayerPerspective(mapMin, playerEncoded);
            InitializeVisibility(ref playerPerspective, isFogOfWar);

            switch (gameBotType)
            {
                case GameBotType.MonteCarloTreeSearchBot:
                    return new MonteCarloTreeSearchBot(playerPerspective, difficulty, isFogOfWar,
                        restrictions);
                case GameBotType.AggressiveBot:
                    return new AggressiveBot(playerPerspective, difficulty,
                        isFogOfWar, restrictions);
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameBotType), gameBotType, null);
            }
        }

        /// <summary>
        /// Initializes visibility of all regions based on <see cref="PlayerPerspective.PlayerId"/>.
        /// </summary>
        /// <param name="playerPerspective"></param>
        /// <param name="isFogOfWar"></param>
        internal void InitializeVisibility(ref PlayerPerspective playerPerspective, bool isFogOfWar)
        {
            if (!isFogOfWar)
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
                    ref var regionMin = ref playerPerspective.MapMin.RegionsMin[index];

                    // region is mine or neighbour to my region => it is visible
                    if (playerPerspective.IsRegionMine(regionMin)
                        || playerPerspective.IsNeighbourToAnyMyRegion(regionMin))
                    {
                        regionMin.IsVisible = true;
                    }
                    else
                    {
                        regionMin.IsVisible = false;
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
        internal MapMin CreateMapForBot(MapMin map, out IdsMappingDictionary regionIdsMappingDictionary,
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
        internal MapMin CreateMapForBot(RegionMin[] regionsMin, SuperRegionMin[] superRegionsMin,
            out IdsMappingDictionary regionIdsMappingDictionary, out IdsMappingDictionary superRegionsIdsMappingDictionary)
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