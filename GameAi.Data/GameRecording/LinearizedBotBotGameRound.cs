namespace GameAi.Data.GameRecording
{
    using System.Collections.Generic;

    public class LinearizedBotBotGameRound : LinearizedBotRound
    {
        public ICollection<BotDeployment> Deployments { get; set; }
        public ICollection<BotAttack> Attacks
            { get; set; }
    }
}