namespace Communication.CommandHandling.Tokens
{
    using Shared;

    public class PlaceArmiesRequestToken : ICommandToken
    {
        public CommandTokenType CommandTokenType { get; }

        public int TimeOut { get; }

        public PlaceArmiesRequestToken(int timeOut = 0)
        {
            TimeOut = timeOut;
        }
    }
}