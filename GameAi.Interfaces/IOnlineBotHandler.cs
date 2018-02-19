namespace GameAi.Interfaces
{
    using System.Threading.Tasks;

    public interface IOnlineBotHandler<TBestMove>
    {
        TBestMove GetCurrentBestMove();
        Task<TBestMove> FindBestMoveAsync();
    }
}