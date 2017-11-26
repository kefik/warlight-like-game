namespace Communication.CommandHandling.Tokens
{
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

        public int TimeOut { get; }

        public AttackRequestToken(int timeOut = 0)
        {
            TimeOut = timeOut;
        }
    }
}