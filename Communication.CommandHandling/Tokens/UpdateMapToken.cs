namespace Communication.CommandHandling.Tokens
{
    using System.Collections.Generic;
    using Shared;

    /// <summary>
    /// Token containing information about changes happening between turns.
    /// </summary>
    public class UpdateMapToken : ICommandToken
    {
        public ICollection<(int RegionId, int Owner, int Army)> Changes { get; }

        public UpdateMapToken(ICollection<(int RegionId, int Owner, int Army)> changes)
        {
            Changes = changes;
        }
    }
}