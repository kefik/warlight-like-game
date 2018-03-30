namespace GameAi.Data.GameRecording
{
    using System.Collections.Generic;
    using System.Linq;

    public class BotGameBeginningTurn : BotTurn
    {
        public IList<int> SeizedRegionsIds { get; set; } = new List<int>();

        public BotGameBeginningTurn(int playerId) : base(playerId)
        {
        }

        public override bool Equals(BotTurn other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }
            if (other.GetType() != typeof(BotGameBeginningTurn))
            {
                return false;
            }

            BotGameBeginningTurn otherTurn =
                (BotGameBeginningTurn) other;
            var otherSeizes = otherTurn.SeizedRegionsIds;

            for (int i = 0; i < SeizedRegionsIds.Count; i++)
            {
                if (SeizedRegionsIds[i] != otherSeizes[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int sum = SeizedRegionsIds.Sum();
            return sum;
        }
    }
}