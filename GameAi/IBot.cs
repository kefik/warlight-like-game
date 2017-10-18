namespace GameAi
{
    /// <summary>
    /// Represents interface every bot must have
    /// </summary>
    /// <typeparam name="TBestMove">Type of best move.</typeparam>
    public interface IBot<out TBestMove>
    {
        /// <summary>
        /// Finds the best move for the player.
        /// </summary>
        /// <returns></returns>
        TBestMove FindBestMove();
    }
}