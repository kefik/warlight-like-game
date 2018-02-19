﻿namespace GameAi.Interfaces
{
    public interface IOnlineBot<TBestMove> : IBot<TBestMove>
    {
        TBestMove CurrentBestMove { get; }
    }
}