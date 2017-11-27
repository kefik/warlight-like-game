namespace TheAiGames.CommunicationObjects.TranslationUnits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Communication.CommandHandling.Tokens.SetupMap;
    using Communication.Shared;

    public class SetupMapTranslationUnit
    {
        private const string Regions = "regions";
        private const string SuperRegions = "super_regions";
        private const string Neighbours = "neighbors";
        
        public ICommandToken Translate(IEnumerable<string> tokens)
        {
            switch (tokens.First())
            {
                case SuperRegions:
                    return CreateSuperRegionsToken(tokens.Skip(1));
                case Regions:
                    return CreateRegionsToken(tokens.Skip(1));
            }

            throw new NotImplementedException();
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
    }
}