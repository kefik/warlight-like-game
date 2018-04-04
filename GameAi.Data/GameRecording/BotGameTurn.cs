namespace GameAi.Data.GameRecording
{
    using System.Collections.Generic;

    public class BotGameTurn : BotTurn
    {
        public IList<BotDeployment> Deployments { get; set; }
        public IList<BotAttack> Attacks { get; set; }

        public BotGameTurn(int playerId) : base(playerId)
        {
            Deployments = new List<BotDeployment>();
            Attacks = new List<BotAttack>();
        }

        public override bool Equals(BotTurn other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (other.GetType() != typeof(BotGameTurn))
            {
                return false;
            }

            BotGameTurn otherTurn = (BotGameTurn) other;
            // assert deployments
            var otherDeployments = otherTurn.Deployments;
            // check length
            if (otherDeployments.Count != Deployments.Count)
            {
                return false;
            }
            for (int i = 0; i < Deployments.Count; i++)
            {
                if (Deployments[i] != otherDeployments[i])
                {
                    return false;
                }
            }

            // assert attacks
            var otherAttacks = otherTurn.Attacks;
            if (otherAttacks.Count != Attacks.Count)
            {
                return false;
            }
            for (int i = 0; i < Attacks.Count; i++)
            {
                if (Attacks[i] != otherAttacks[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int sum = 0;
            foreach (BotDeployment botDeployment in Deployments)
            {
                sum += botDeployment.GetHashCode();
            }
            foreach (BotAttack botAttack in Attacks)
            {
                sum += botAttack.GetHashCode();
            }
            return sum;
        }
    }
}