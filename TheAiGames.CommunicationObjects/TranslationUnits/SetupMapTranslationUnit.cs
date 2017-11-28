namespace TheAiGames.CommunicationObjects.TranslationUnits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Communication.CommandHandling.Tokens.SetupMap;
    using Communication.Shared;

    /// <summary>
    /// Translates setup.
    /// </summary>
    internal class SetupMapTranslationUnit
    {
        private const string Regions = "regions";
        private const string SuperRegions = "super_regions";
        private const string Neighbours = "neighbors";
        private const string Wastelands = "wastelands";

        public ICommandToken Translate(IEnumerable<string> tokens)
        {
            switch (tokens.First())
            {
                case SuperRegions:
                    return CreateSuperRegionsToken(tokens.Skip(1));
                case Regions:
                    return CreateRegionsToken(tokens.Skip(1));
                case Neighbours:
                    return CreateNeighboursToken(tokens.Skip(1));
                case Wastelands:
                    return CreateWastelandsToken(tokens.Skip(1));
                default:
                    throw new ArgumentOutOfRangeException(nameof(tokens));
            }
        }

        private SetupSuperRegionsToken CreateSuperRegionsToken(IEnumerable<string> tokens)
        {
            var changes = new List<(int SuperRegionId, int BonusArmy)>();

            bool odd = true;
            int superRegionId = -1, bonusArmy = -1;

            // each odd == superRegionId, even = bonusArmy of the region
            foreach (var token in tokens)
            {
                if (odd)
                {
                    superRegionId = int.Parse(token);
                }
                else
                {
                    bonusArmy = int.Parse(token);

                    changes.Add((superRegionId, bonusArmy));
                }

                odd = !odd;
            }

            return new SetupSuperRegionsToken(changes);
        }

        private SetupRegionsToken CreateRegionsToken(IEnumerable<string> tokens)
        {
            var changes = new List<(int RegionId, int SuperRegionId)>();

            bool odd = true;
            int regionId = -1, superRegionId = -1;

            foreach (var token in tokens)
            {
                if (odd)
                {
                    regionId = int.Parse(token);
                }
                else
                {
                    superRegionId = int.Parse(token);

                    changes.Add((regionId, superRegionId));
                }

                odd = !odd;
            }

            return new SetupRegionsToken(changes);
        }

        private SetupNeighboursToken CreateNeighboursToken(IEnumerable<string> tokens)
        {
            bool odd = true;

            var neighbourRelationCollection = new List<(int RegionId, IList<int> NeighbourIds)>();

            int regionId = -1;

            // odd == regionId, even == symmetric list of neighbours separated by comma
            // for our framework we need to add symmetricity
            foreach (var token in tokens)
            {
                if (odd)
                {
                    regionId = int.Parse(token);
                }
                else
                {
                    int[] neighbours = token.Split(',').Select(int.Parse).ToArray();

                    neighbourRelationCollection.Add((regionId, neighbours));
                }

                odd = !odd;
            }

            AddSymmetricRelations(neighbourRelationCollection);

            return new SetupNeighboursToken(neighbourRelationCollection);
        }

        /// <summary>
        /// Adds symmetric relation from previous non-symmetric graph of neighbours.
        /// </summary>
        /// <param name="regionsWithNeighbours"></param>
        private void AddSymmetricRelations(IList<(int RegionId, IList<int> NeighbourIds)> regionsWithNeighbours)
        {
            // all neighbours ids
            var allNeighboursIds = (from region in regionsWithNeighbours
                                    select region.NeighbourIds into neighbours
                                    from neighbour in neighbours
                                    select neighbour).Distinct();

            foreach (int neighbourId in allNeighboursIds)
            {
                // if there is neighbours that is not defined as RegionId, add it
                if (regionsWithNeighbours.All(x => x.RegionId != neighbourId))
                {
                    regionsWithNeighbours.Add((neighbourId, new List<int>()));
                }
            }

            // now add symmetricity (if region X is neighbour of Y, then Y is neighbour of X)
            for (int i = 0; i < regionsWithNeighbours.Count; i++)
            {
                var regionWithNeighbours = regionsWithNeighbours[i];

                // neighbours of current region defined from the perspective of other regions
                var allRegionNeighbours = regionWithNeighbours.NeighbourIds.Concat(
                    from otherRegions in regionsWithNeighbours
                    where otherRegions.RegionId != regionWithNeighbours.RegionId
                    where otherRegions.NeighbourIds.Contains(regionWithNeighbours.RegionId)
                    select otherRegions.RegionId);

                // currently defined neighbours united with those regions that have current region defined as neighbour = new neighbours
                regionWithNeighbours.NeighbourIds = regionWithNeighbours.NeighbourIds.Union(allRegionNeighbours).ToList();
            }
        }

        private SetupWastelandsToken CreateWastelandsToken(IEnumerable<string> tokens)
        {
            var regionsIds = new List<int>();

            foreach (string token in tokens)
            {
                regionsIds.Add(int.Parse(token));
            }

            return new SetupWastelandsToken(regionsIds);
        }
    }
}