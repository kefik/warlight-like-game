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
    }
}