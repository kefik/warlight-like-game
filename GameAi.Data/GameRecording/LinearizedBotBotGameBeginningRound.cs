namespace GameAi.Data.GameRecording
{
    using System.Collections.Generic;

    public class LinearizedBotBotGameBeginningRound : LinearizedBotRound
    {
        public ICollection<(byte SeizingPlayerId, int RegionId)> SeizedRegionsIds { get; set; } = new List<(byte SeizingPlayerId, int RegionId)>();
    }
}