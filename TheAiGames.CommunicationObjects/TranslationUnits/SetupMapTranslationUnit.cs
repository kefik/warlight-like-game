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


        public ICommandToken Translate(IEnumerable<string> tokens)
        {
            switch (tokens.First())
            {
                case SuperRegions:
                    return CreateSuperRegionsToken(tokens.Skip(1));
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

            throw new NotImplementedException();
        }
    }
}