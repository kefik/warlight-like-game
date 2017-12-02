﻿namespace Communication.CommandHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameAi;
    using Shared;
    using Tokens;
    using Tokens.SetupMap;

    public class MapController
    {
        private readonly bool isFogOfWar;
        private readonly int defaultArmy;

        private Map map;

        public bool HasStarted { get; private set; }

        private SetupSuperRegionsToken setupSuperRegionsToken;
        private SetupRegionsToken setupRegionsToken;
        private SetupNeighboursToken setupNeighboursToken;
        private SetupWastelandsToken setupWastelandsToken;

        public MapController(bool isFogOfWar, int defaultArmy = 2)
        {
            this.isFogOfWar = isFogOfWar;
            this.defaultArmy = defaultArmy;
        }

        /// <summary>
        /// Adds something to map settings.
        /// </summary>
        /// <remarks>
        /// Token in parameter must be Setup token.
        /// </remarks>
        /// <param name="commandToken">Token.</param>
        public void SetupMap(ISetupMapToken commandToken)
        {
            if (HasStarted)
            {
                throw new ArgumentException($"The game has already started, you cannot setup map.");
            }

            switch (commandToken.CommandTokenType)
            {
                case CommandTokenType.SetupSuperRegions:
                    setupSuperRegionsToken = (SetupSuperRegionsToken)commandToken;
                    break;
                case CommandTokenType.SetupRegions:
                    setupRegionsToken = (SetupRegionsToken)commandToken;
                    break;
                case CommandTokenType.SetupNeighbours:
                    setupNeighboursToken = (SetupNeighboursToken)commandToken;
                    break;
                case CommandTokenType.SetupWastelands:
                    setupWastelandsToken = (SetupWastelandsToken)commandToken;
                    break;
                default:
                    throw new ArgumentException($"When calling {nameof(SetupMap)}, its argument {nameof(commandToken)} must be of Setup type.");
            }
        }

        /// <summary>
        /// Updates map based on token passed in parameter.
        /// </summary>
        /// <param name="commandToken"></param>
        public void UpdateMap(UpdateMapToken commandToken)
        {
            var updateMapToken = commandToken;

            foreach (var change in updateMapToken.Changes)
            {
                // get the region
                var region = map.RegionsMin.First(x => x.Id == change.RegionId);

                // update the region
                region.Army = change.Army;
                region.OwnerEncoded = (byte)(change.Owner);
            }
        }

        /// <summary>
        /// Gets map that was setted up by tokens.
        /// </summary>
        /// <returns></returns>
        public Map GetMap()
        {
            if (!HasStarted)
            {
                throw new ArgumentException($"You cannot get map because the game has not started.");
            }

            return map;
        }

        /// <summary>
        /// Runs all map settings, starting the game.
        /// </summary>
        public void Start()
        {
            if (HasStarted)
            {
                throw new ArgumentException($"The game has already started.");
            }

            HasStarted = true;

            ICollection<RegionMin> regionsMin = new List<RegionMin>();
            ICollection<SuperRegionMin> superRegionsMin = new List<SuperRegionMin>();
            
            SetupSuperRegions(superRegionsMin);
            
            SetupRegions(regionsMin, superRegionsMin);
            
            SetupNeighbours(regionsMin);

            // add regions to super region
            foreach (SuperRegionMin superRegionMin in superRegionsMin)
            {
                var regionsUnderSuperRegion = regionsMin.Where(x => x.SuperRegion?.Id == superRegionMin.Id);
                superRegionMin.Regions = regionsUnderSuperRegion.ToArray();
            }

            map = new Map(regionsMin.ToArray(), superRegionsMin.ToArray());

            map.ReconstructGraph();
        }

        private void SetupNeighbours(ICollection<RegionMin> regionsMin)
        {
            foreach (var neighbourInitialization in setupNeighboursToken.NeighboursInitialization)
            {
                var region = regionsMin.First(x => x.Id == neighbourInitialization.RegionId);

                var regionNeighbours = regionsMin.Where(x => neighbourInitialization.NeighboursIds.Contains(x.Id));

                region.NeighbourRegions = regionNeighbours.ToArray();
            }
        }

        private void SetupRegions(ICollection<RegionMin> regionsMin, ICollection<SuperRegionMin> superRegionsMin)
        {
            foreach (var setupRegionsInstruction in setupRegionsToken.SetupRegionsInstructions)
            {
                (int regionId, int superRegionId) = setupRegionsInstruction;

                if (regionsMin.Any(x => x.Id == regionId))
                {
                    throw new ArgumentException($"Region {nameof(regionId)} has already been initialized.");
                }

                var superRegion = superRegionsMin.FirstOrDefault(x => x.Id == superRegionId);

                if (superRegion == null)
                {
                    throw new ArgumentException($"SuperRegions must be initialized before regions");
                }

                RegionMin regionMin;
                if (setupWastelandsToken == null || !setupWastelandsToken.Regions.Contains(regionId))
                {
                    regionMin = new RegionMin(regionId, superRegion, defaultArmy, isFogOfWar);
                }
                else
                {
                    regionMin = new RegionMin(regionId, superRegion, defaultArmy, isFogOfWar, true);
                }

                regionsMin.Add(regionMin);
            }
        }

        private void SetupSuperRegions(ICollection<SuperRegionMin> superRegionsMin)
        {
            foreach (var initialChange in setupSuperRegionsToken.InitialChanges)
            {
                if (superRegionsMin.Any(x => x.Id == initialChange.SuperRegionId))
                {
                    throw new ArgumentException(
                        $"SuperRegion with id {initialChange.SuperRegionId} has already been initialized.");
                }

                superRegionsMin.Add(new SuperRegionMin(initialChange.SuperRegionId, initialChange.BonusArmy));
            }
        }
    }
}