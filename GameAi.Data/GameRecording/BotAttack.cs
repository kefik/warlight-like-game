namespace GameAi.Data.GameRecording
{
    using System;
    public struct BotAttack : IEquatable<BotAttack>
    {
        public int AttackingPlayerId { get; }
        public int AttackingRegionId { get; }
        public int AttackingArmy { get; }
        public int DefendingRegionId { get; }

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

        public bool Equals(BotAttack other)
        {
            return AttackingPlayerId == other.AttackingPlayerId
                && AttackingRegionId == other.AttackingRegionId
                && AttackingArmy == other.AttackingArmy
                && DefendingRegionId == other.DefendingRegionId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is BotAttack && Equals((BotAttack) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = AttackingPlayerId;
                hashCode = (hashCode * 397) ^ AttackingRegionId;
                hashCode = (hashCode * 397) ^ AttackingArmy;
                hashCode = (hashCode * 397) ^ DefendingRegionId;
                return hashCode;
            }
        }

        public static bool operator ==(BotAttack left,
            BotAttack right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BotAttack left, BotAttack right)
        {
            return !(left == right);
        }
    }
}