namespace Communication.CommandHandling.Tokens
{
    using System;
    using Shared;

    public class PlaceArmiesRequestToken : ICommandToken
    {
        public TimeSpan? Timeout { get; }

        public PlaceArmiesRequestToken(TimeSpan? timeOut = null)
        {
            Timeout = timeOut;
        }
    }
}