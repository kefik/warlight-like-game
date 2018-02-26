namespace GameAi.Interfaces
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents interface every bot must have
    /// </summary>
    /// <typeparam name="TBestMove">Type of best move.</typeparam>
    public interface IBot<TBestMove>
    {
        /// <summary>
        /// Reports whether the bot evaluation can start
        /// </summary>
        bool CanStartEvaluation { get; }

        /// <summary>
        /// Finds the best move for the player.
        /// </summary>
        /// <returns></returns>
        Task<TBestMove> FindBestMoveAsync();

        /// <summary>
        /// Updates map of bot based on parameter.
        /// </summary>
        void UpdateMap();

        /// <summary>
        /// Stops searching for the best move.
        /// </summary>
        /// <exception cref="NotSupportedException">Reports that given bot doesn't support this kind of behaviour.</exception>
        void StopEvaluation();
    }
}