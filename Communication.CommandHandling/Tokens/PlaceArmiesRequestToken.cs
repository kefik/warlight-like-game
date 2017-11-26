namespace Communication.CommandHandling.Tokens
{
    using Shared;

    public class PlaceArmiesRequestToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get
            {
                return CommandTokenType.PlaceArmiesRequest;
            }
        }

        public int TimeOut { get; }

        public PlaceArmiesRequestToken(int timeOut = 0)
        {
            TimeOut = timeOut;
        }
    }
}