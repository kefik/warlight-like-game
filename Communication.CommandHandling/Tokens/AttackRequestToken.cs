namespace Communication.CommandHandling.Tokens
{
    using Shared;

    public class AttackRequestToken : ICommandToken
    {
        public CommandTokenType CommandTokenType { get; }

        public int TimeOut { get; }

        public AttackRequestToken(int timeOut = 0)
        {
            TimeOut = timeOut;
        }
    }
}