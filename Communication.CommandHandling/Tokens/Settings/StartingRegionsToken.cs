namespace Communication.CommandHandling.Tokens.Settings
{
    using System.Collections.Generic;
    using Shared;

    public class StartingRegionsToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.SettingsStartingRegions; }
        }

        public ICollection<int> RegionIds { get; }

        public StartingRegionsToken(ICollection<int> regionIds)
        {
            RegionIds = regionIds;
        }
    }
}