namespace GameAi.Data.GameRecording
{
    using System.Collections.Generic;

    public class BotGameTurn : BotTurn
    {
        public ICollection<(int RegionId, int Army)> Deployments { get; set; }
        public ICollection<(int AttackingRegionId, int AttackingArmy, int DefendingRegionId)> Attacks { get; set; }

        public BotGameTurn(int playerId) : base(playerId)
        {
            Deployments = new List<(int RegionId, int Army)>();
        }
    }
}