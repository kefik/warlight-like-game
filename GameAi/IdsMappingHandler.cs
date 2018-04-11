#if DEBUG
#define LOG_MAPPING_HANDLER
#endif

namespace GameAi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Data.EvaluationStructures;
    using Data.GameRecording;
    using Data.Restrictions;
    using FormatConverters;

    /// <summary>
    /// Component handling issues related with mapping regions ids.
    /// </summary>
    public class IdsMappingHandler
    {
        private readonly IdsMappingDictionary
            regionIdsMappingDictionary;

        private readonly IdsMappingDictionary
            superRegionsIdsMappingDictionary;

        private readonly IdsMappingDictionary
            playersIdsMappingDictionary;

        public IdsMappingHandler(IEnumerable<int> regionsIds,
            IEnumerable<int> supersRegionIds,
            IEnumerable<int> playersIds)
        {
            regionIdsMappingDictionary = new IdsMappingDictionary();
            superRegionsIdsMappingDictionary =
                new IdsMappingDictionary();
            playersIdsMappingDictionary = new IdsMappingDictionary();

            foreach (var regionId in regionsIds)
            {
                regionIdsMappingDictionary
                    .GetMappedIdOrInsert(regionId);
            }

            foreach (var superRegionId in supersRegionIds)
            {
                superRegionsIdsMappingDictionary.GetMappedIdOrInsert(
                    superRegionId);
            }

            // TODO: rather use -1 for no conflicts with players
            playersIdsMappingDictionary.GetMappedIdOrInsert(0);
            foreach (int playersId in playersIds)
            {
                playersIdsMappingDictionary
                    .GetMappedIdOrInsert(playersId);
            }
        }

        /// <summary>
        /// Creates MapMin instance. Parameters must relate to ids
        /// passed in constructor, otherwise the method will fail.
        /// </summary>
        /// <returns></returns>
        public MapMin TranslateToNew(MapMin originalMap)
        {
            return TranslateToNew(originalMap.RegionsMin,
                originalMap.SuperRegionsMin);
        }

        /// <summary>
        /// Creates MapMin instance. Parameters must relate to ids
        /// passed in constructor, otherwise the method will fail.
        /// </summary>
        /// <param name="regionsMin"></param>
        /// <param name="superRegionsMin"></param>
        /// <returns></returns>
        public MapMin TranslateToNew(RegionMin[] regionsMin,
            SuperRegionMin[] superRegionsMin)
        {
            // we dont want to change original structures => deep copy
            var tempMapMin = new MapMin(regionsMin, superRegionsMin)
                .DeepCopy();
            regionsMin = tempMapMin.RegionsMin.OrderBy(x => x.Id)
                .ToArray();
            superRegionsMin = tempMapMin.SuperRegionsMin
                .OrderBy(x => x.Id).ToArray();

            // so far we expect Ids to be ordered and unique
            // now we have to shuffle it to minimal values => e.g. 2, 5, 7 => 0, 1, 2

            for (int i = 0; i < regionsMin.Length; i++)
            {
                ref RegionMin region = ref regionsMin[i];

                int mappedId =
                    regionIdsMappingDictionary.GetNewId(regionsMin[i]
                        .Id);

                byte ownerId =
                    (byte)playersIdsMappingDictionary.GetNewId(region
                        .OwnerId);

                RemapId(regionsMin, superRegionsMin, ref region,
                    mappedId);

                region.OwnerId = ownerId;
                region.Id = mappedId;
            }

            // do the same with super regions
            for (int i = 0; i < superRegionsMin.Length; i++)
            {
                ref SuperRegionMin superRegion =
                    ref superRegionsMin[i];

                int mappedId =
                    superRegionsIdsMappingDictionary.GetNewId(
                        superRegionsMin[i].Id);

                byte ownerId = (byte)playersIdsMappingDictionary.GetNewId(superRegion.OwnerId);

                RemapId(regionsMin, ref superRegion, mappedId);

                superRegion.Id = mappedId;
                superRegion.OwnerId = ownerId;
            }

            return new MapMin(regionsMin, superRegionsMin);
        }

        private void RemapId(RegionMin[] regionsMin,
            SuperRegionMin[] superRegionsMin, ref RegionMin regionMin,
            int newRegionId)
        {
            var neighboursIds = regionMin.NeighbourRegionsIds;

            var neighbours =
                regionsMin.Where(x => neighboursIds.Contains(x.Id));

            // remap neighbours
            // for each neighbour change ID of regionMin to newId
            foreach (var neighbour in neighbours)
            {
                var neighbourNeighbours =
                    neighbour.NeighbourRegionsIds;

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

            var superRegion =
                superRegionsMin.First(x => x.Id == superRegionId);
            int[] superRegionRegionsIds = superRegion.RegionsIds;

            for (int i = 0; i < superRegionRegionsIds.Length; i++)
            {
                if (superRegionRegionsIds[i] == regionMin.Id)
                {
                    superRegionRegionsIds[i] = newRegionId;
                }
            }
        }

        private void RemapId(RegionMin[] regionsMin,
            ref SuperRegionMin currentSuperRegionMin,
            int newSuperRegionId)
        {
            int oldSuperRegionId = currentSuperRegionMin.Id;

            // remap super regions regions
            var superRegionsRegions =
                regionsMin.Where(
                    x => x.SuperRegionId == oldSuperRegionId);

            foreach (RegionMin superRegionsRegion in
                superRegionsRegions)
            {
                RegionMin regionsRegion = superRegionsRegion;

                regionsRegion.SuperRegionId = newSuperRegionId;
            }
        }

        /// <summary>
        /// Translates restrictions, remapping from old region and super region IDs to new.
        /// </summary>
        /// <param name="restrictions"></param>
        /// <returns></returns>
        public Restrictions TranslateToNew(Restrictions restrictions)
        {
            var newRestrictions = new Restrictions();

            foreach (GameBeginningRestriction gameBeginningRestriction
                in restrictions.GameBeginningRestrictions)
            {
                newRestrictions.GameBeginningRestrictions.Add(
                    new GameBeginningRestriction()
                    {
                        PlayerId = playersIdsMappingDictionary.GetNewId(gameBeginningRestriction.PlayerId),
                        RegionsPlayerCanChooseCount =
                            gameBeginningRestriction
                                .RegionsPlayerCanChooseCount,
                        RestrictedRegions = gameBeginningRestriction
                            .RestrictedRegions
                            .Select(x => regionIdsMappingDictionary
                                .GetNewId(x)).ToList()
                    });
            }

            return newRestrictions;
        }

        /// <summary>
        /// Translates restrictions, remapping from new region and super region IDs to old.
        /// </summary>
        /// <param name="turn"></param>
        /// <returns></returns>
        public BotTurn TranslateToOriginal(BotTurn turn)
        {
            switch (turn)
            {
                case BotGameBeginningTurn gameBeginningTurn:
                    var translatedRegions =
                        gameBeginningTurn.SeizedRegionsIds
                            .Select(x => regionIdsMappingDictionary
                                .GetOriginalId(x)).ToList();

                    return new BotGameBeginningTurn(
                        playersIdsMappingDictionary.GetOriginalId(turn
                            .PlayerId))
                    {
                        SeizedRegionsIds = translatedRegions
                    };
                case BotGameTurn gameTurn:
                    var attacks = gameTurn.Attacks
                        .Select(
                            x => new BotAttack(
                                playersIdsMappingDictionary
                                    .GetOriginalId(
                                        x.AttackingPlayerId),
                                regionIdsMappingDictionary
                                    .GetOriginalId(
                                        x.AttackingRegionId),
                                x.AttackingArmy,
                                regionIdsMappingDictionary
                                    .GetOriginalId(
                                        x.DefendingRegionId)))
                        .ToList();

                    var deploys = gameTurn.Deployments
                        .Select(
                            x => new BotDeployment(
                                regionIdsMappingDictionary
                                    .GetOriginalId(x.RegionId),
                                x.Army, playersIdsMappingDictionary.GetOriginalId(x.DeployingPlayerId)))
                        .ToList();

                    return new BotGameTurn(playersIdsMappingDictionary
                        .GetOriginalId(turn.PlayerId))
                    {
                        Attacks = attacks,
                        Deployments = deploys
                    };
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}