namespace Communication.CommandHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameAi;
    using GameAi.EvaluationStructures;
    using Shared;
    using Tokens;
    using Tokens.SetupMap;

    public class MapController
    {
        private readonly int defaultArmy;
        
        private MapMin map;

        public bool HasStarted { get; private set; }

        private SetupSuperRegionsToken setupSuperRegionsToken;
        private SetupRegionsToken setupRegionsToken;
        private SetupNeighboursToken setupNeighboursToken;
        private SetupWastelandsToken setupWastelandsToken;

        public MapController(int defaultArmy = 2)
        {
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

            switch (commandToken)
            {
                case SetupSuperRegionsToken token:
                    setupSuperRegionsToken = token;
                    break;
                case SetupRegionsToken token:
                    setupRegionsToken = token;
                    break;
                case SetupNeighboursToken token:
                    setupNeighboursToken = token;
                    break;
                case SetupWastelandsToken token:
                    setupWastelandsToken = token;
                    break;
                default:
                    throw new ArgumentException($"When calling {nameof(SetupMap)}, its argument {nameof(commandToken)} must be of Setup type.");
            }
        }

        private ref RegionMin GetRegionMin(int regionId)
        {
            var regionsMin = map.RegionsMin;
            for (int i = 0; i < regionsMin.Length; i++)
            {
                if (regionsMin[i].Id == regionId)
                {
                    return ref regionsMin[i];
                }
            }
            
            throw new ArgumentException($"Region with id {regionId} does not exist.");
        }

        /// <summary>
        /// Updates map based on token passed in parameter.
        /// </summary>
        /// <param name="commandToken"></param>
        public void UpdateMap(UpdateMapToken commandToken)
        {
            var updateMapToken = commandToken;

            foreach (var (regionId, owner, army) in updateMapToken.Changes)
            {
                // get the region
                ref var region = ref GetRegionMin(regionId);

                // update the region
                region.Army = army;
                region.OwnerId = (byte)(owner);
            }
        }

        /// <summary>
        /// Gets map that was setted up by tokens.
        /// </summary>
        /// <returns></returns>
        public MapMin GetMap()
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

            var regionsMin = new List<RegionMin>();
            var superRegionsMin = new List<SuperRegionMin>();
            
            SetupSuperRegions(superRegionsMin);
            
            SetupRegions(regionsMin, superRegionsMin);
            
            SetupNeighbours(regionsMin);

            // add regions to super region
            for (int index = 0; index < superRegionsMin.Count; index++)
            {
                SuperRegionMin superRegionMin = superRegionsMin[index];

                var regionsUnderSuperRegionIds = regionsMin.Where(x => x.SuperRegionId == superRegionMin.Id).Select(x => x.Id);
                superRegionMin.RegionsIds = regionsUnderSuperRegionIds.ToArray();

                superRegionsMin[index] = superRegionMin;
            }
            
            map = new MapMin(regionsMin.ToArray(), superRegionsMin.ToArray());
        }

        private void SetupNeighbours(ICollection<RegionMin> regionsMin)
        {
            foreach (var (regionId, neighboursIds) in setupNeighboursToken.NeighboursInitialization)
            {
                var region = regionsMin.First(x => x.Id == regionId);
                
                region.NeighbourRegionsIds = neighboursIds.ToArray();
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

                RegionMin regionMin;
                if (setupWastelandsToken == null || !setupWastelandsToken.Regions.Contains(regionId))
                {
                    regionMin = new RegionMin(regionId, superRegionId, defaultArmy);
                }
                else
                {
                    regionMin = new RegionMin(regionId, superRegionId, defaultArmy, isWasteland: true);
                }

                regionsMin.Add(regionMin);
            }
        }

        private void SetupSuperRegions(ICollection<SuperRegionMin> superRegionsMin)
        {
            foreach (var (superRegionId, bonusArmy) in setupSuperRegionsToken.InitialChanges)
            {
                if (superRegionsMin.Any(x => x.Id == superRegionId))
                {
                    throw new ArgumentException(
                        $"SuperRegion with id {superRegionId} has already been initialized.");
                }

                superRegionsMin.Add(new SuperRegionMin(superRegionId, bonusArmy));
            }
        }
    }
}