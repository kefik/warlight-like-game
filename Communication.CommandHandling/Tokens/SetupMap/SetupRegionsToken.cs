namespace Communication.CommandHandling.Tokens.SetupMap
{
    using System.Collections.Generic;
    using Shared;

    /// <summary>
    /// Token which contains region inicialization data.
    /// </summary>
    public class SetupRegionsToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.SetupRegions; }
        }

        public ICollection<(int RegionId, int SuperRegionId)> SetupRegionsInstructions { get; }

        public SetupRegionsToken(ICollection<(int RegionId, int SuperRegionId)> setupRegionsInstructions)
        {
            SetupRegionsInstructions = setupRegionsInstructions;
        }
    }
}