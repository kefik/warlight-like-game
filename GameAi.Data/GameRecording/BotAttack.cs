namespace GameAi.Data.GameRecording
{
    public struct BotAttack
    {
        public int AttackingPlayerId { get; set; }
        public int AttackingRegionId { get; set; }
        public int AttackingArmy { get; set; }
        public int DefendingRegionId { get; set; }

        public BotAttack(int attackingPlayerId, int attackingRegionId,
            int attackingArmy, int defendingRegionId)
        {
            AttackingPlayerId = attackingPlayerId;
            AttackingRegionId = attackingRegionId;
            AttackingArmy = attackingArmy;
            DefendingRegionId = defendingRegionId;
        }

        public override string ToString()
        {
            return $"Attacker: {AttackingPlayerId}, " +
                   $"{AttackingRegionId} -> " +
                   $"{DefendingRegionId}, " +
                   $"{AttackingArmy}";
        }
    }
}