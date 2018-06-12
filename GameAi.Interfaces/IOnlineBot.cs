namespace GameAi.Interfaces
{
    using System.Collections.Generic;
    using Data.GameRecording;

    public interface IOnlineBot<TBestMove> : IBot<TBestMove>
    {
        TBestMove GetCurrentBestMove();

        void UseFixedDeploy(
            IEnumerable<BotDeployment> deploymentsToUse);
    }
}