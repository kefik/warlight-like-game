namespace GameAi.Data.GameRecording
{
    using System.Collections.Generic;

    public class BotGameTurn : BotTurn
    {
        public IList<(int RegionId, int Army, int DeployingPlayerId)> Deployments { get; set; }
        public IList<(int AttackingPlayerId, int AttackingRegionId,
            int AttackingArmy, int DefendingRegionId)> Attacks { get; set; }

        public BotGameTurn(int playerId) : base(playerId)
        {
            Deployments = new List<(int RegionId, int Army, int DeployingPlayerId)>();
            Attacks = new List<(int AttackingPlayerId, int AttackingRegionId,
                int AttackingArmy, int DefendingRegionId)>();
        }
    }
}