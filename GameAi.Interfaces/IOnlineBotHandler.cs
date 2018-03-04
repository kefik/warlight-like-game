namespace GameAi.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IOnlineBotHandler<TBestMove>
    {
        TBestMove GetCurrentBestMove();
        /// <summary>
        /// Finds asynchronously best move in specified time.
        /// </summary>
        /// <returns></returns>
        Task<TBestMove> FindBestMoveAsync();

        void StopEvaluation();

        Task StopEvaluation(TimeSpan timeSpan);
    }
}