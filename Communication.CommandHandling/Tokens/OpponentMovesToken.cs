namespace Communication.CommandHandling.Tokens
{
    using Shared;

    /// <summary>
    /// Token representing visible moves opponent has done in consecutive order.
    /// </summary>
    public class OpponentMovesToken : ICommandToken
    {
        public CommandTokenType CommandTokenType
        {
            get
            {
                return CommandTokenType.OpponentMoves;
            }
        }
    }
}