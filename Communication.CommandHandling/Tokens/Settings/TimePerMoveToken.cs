namespace Communication.CommandHandling.Tokens.Settings
{
    using System;
    using Shared;
    public class TimePerMoveToken : ICommandToken
    {
        public TimeSpan Time { get; }

        public TimePerMoveToken(TimeSpan time)
        {
            Time = time;
        }
    }
}