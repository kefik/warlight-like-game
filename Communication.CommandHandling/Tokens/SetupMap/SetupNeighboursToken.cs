namespace Communication.CommandHandling.Tokens.SetupMap
{
    using System.Collections.Generic;
    using Shared;

    /// <summary>
    /// Token which contains neighbour inicialization data.
    /// </summary>
    public class SetupNeighboursToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get { return CommandTokenType.SetupNeighbours; }
        }

        public ICollection<(int RegionId, int[] NeighboursIds)> NeighboursInitialization { get; }

        public SetupNeighboursToken(ICollection<(int RegionId, int[] NeighboursIds)> neighboursInitialization)
        {
            NeighboursInitialization = neighboursInitialization;
        }
    }
}