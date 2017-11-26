namespace Communication.CommandHandling
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
        private bool isFogOfWar;

        private Map map;

        public bool HasStarted { get; private set; }

        private SetupSuperRegionsToken setupSuperRegionsToken;
        private SetupRegionsToken setupRegionsToken;
        private SetupNeighboursToken setupNeighboursToken;
        private SetupWastelandsToken setupWastelandsToken;

        public MapController(bool isFogOfWar)
        {
            this.isFogOfWar = isFogOfWar;
        }

        public void SetupMap(ICommandToken commandToken)
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

        public void UpdateMapToken(ICommandToken commandToken)
        {
            if (commandToken.CommandTokenType != CommandTokenType.UpdateMap)
            {
                throw new ArgumentException($"Invalid argument type {nameof(commandToken)}.");
            }

            var updateMapToken = (UpdateMapToken)commandToken;

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
            return map;
        }

        public void Start()
        {
            if (HasStarted)
            {
                throw new ArgumentException($"The game has already started.");
            }

            HasStarted = true;

            ICollection<RegionMin> regionsMin = new List<RegionMin>();
            ICollection<SuperRegionMin> superRegionsMin = new List<SuperRegionMin>();

            #region Setup SuperRegions
            foreach (var initialChange in setupSuperRegionsToken.InitialChanges)
            {
                if (superRegionsMin.Any(x => x.Id == initialChange.SuperRegionId))
                {
                    throw new ArgumentException($"SuperRegion with id {initialChange.SuperRegionId} has already been initialized.");
                }

                superRegionsMin.Add(new SuperRegionMin(initialChange.SuperRegionId, initialChange.BonusArmy));
            }
            #endregion

            #region Setup Regions
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

                // TODO: 2 fix
                RegionMin regionMin = new RegionMin(regionId, superRegion, 2, isFogOfWar);

                regionsMin.Add(regionMin);
            }
            #endregion

            #region Setup Neighbours
            foreach (var neighbourInitialization in setupNeighboursToken.NeighboursInitialization)
            {
                var region = regionsMin.First(x => x.Id == neighbourInitialization.RegionId);

                var regionNeighbours = regionsMin.Where(x => neighbourInitialization.NeighboursIds.Contains(x.Id));

                region.NeighbourRegions = regionNeighbours.ToArray();
            }
            #endregion

            // add regions to super region
            foreach (SuperRegionMin superRegionMin in superRegionsMin)
            {
                var regionsUnderSuperRegion = regionsMin.Where(x => x.SuperRegion?.Id == superRegionMin.Id);
                superRegionMin.Regions = regionsUnderSuperRegion.ToArray();
            }

            map = new Map(regionsMin.ToArray(), superRegionsMin.ToArray());

            map.ReconstructGraph();
        }
    }
}