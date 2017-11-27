namespace Communication.CommandHandling.Tokens
{
    using System;
    using Shared;

    /// <summary>
    /// Token specifying attack request.
    /// </summary>
    public class AttackRequestToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get
            {
                return CommandTokenType.AttackRequest;
            }
        }

        public TimeSpan? Timeout { get; }

        public AttackRequestToken(TimeSpan? timeout = null)
        {
            Timeout = timeout;
        }
    }
}