namespace Communication.CommandHandling.Tokens.SetupMap
{
    using System.Collections.Generic;
    using Shared;
    public class SetupWastelandsToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.SetupWastelands; }
        }

        public ICollection<int> Regions { get; }

        public SetupWastelandsToken(ICollection<int> regions)
        {
            Regions = regions;
        }
    }
}