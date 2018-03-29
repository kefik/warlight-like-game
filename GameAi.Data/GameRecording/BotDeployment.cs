namespace GameAi.Data.GameRecording
{
    using System;
    public struct BotDeployment : IEquatable<BotDeployment>
    {
        public int RegionId { get;  }
        public int Army { get; }
        public int DeployingPlayerId { get; }

        public BotDeployment(int regionId, int army, int deployingPlayerId)
        {
            RegionId = regionId;
            Army = army;
            DeployingPlayerId = deployingPlayerId;
        }

        public override string ToString()
        {
            return $"Player: {DeployingPlayerId}, Region: {RegionId}, Army: {Army}";
        }

        public bool Equals(BotDeployment other)
        {
            return RegionId == other.RegionId && Army == other.Army && DeployingPlayerId == other.DeployingPlayerId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is BotDeployment && Equals((BotDeployment) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = RegionId;
                hashCode = (hashCode * 397) ^ Army;
                hashCode = (hashCode * 397) ^ DeployingPlayerId;
                return hashCode;
            }
        }
    }
}