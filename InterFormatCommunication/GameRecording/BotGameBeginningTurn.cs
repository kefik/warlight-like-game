namespace InterFormatCommunication.GameRecording
{
    using System.Collections.Generic;

    public class BotGameBeginningTurn : BotTurn
    {
        public ICollection<int> SeizedRegionsIds { get; set; }

        public BotGameBeginningTurn(int playerId) : base(playerId)
        {
        }
    }
}