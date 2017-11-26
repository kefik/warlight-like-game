namespace Communication.CommandHandling.Tokens
{
    using System.Collections.Generic;
    using Shared;

    public class UpdateMapToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get
            {
                return CommandTokenType.UpdateMap;
            }
        }

        public IList<(int RegionId, string Owner, int Army)> Changes { get; }

        public UpdateMapToken(IList<(int RegionId, string Owner, int Army)> changes)
        {
            Changes = changes;
        }
    }
}