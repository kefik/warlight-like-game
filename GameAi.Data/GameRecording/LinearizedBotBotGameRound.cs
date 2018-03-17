namespace GameAi.Data.GameRecording
{
    using System.Collections.Generic;

    public class LinearizedBotBotGameRound : LinearizedBotRound
    {
        public ICollection<(int RegionId, int Army, int DeployingPlayerId)> Deployments { get; set; }
        public ICollection<(int AttackingPlayerId, int AttackingRegionId,
            int AttackingArmy, int DefendingRegionId)> Attacks
            { get; set; }
    }
}