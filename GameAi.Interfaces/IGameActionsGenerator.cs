namespace GameAi.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Class inheriting from this can generate
    /// game action for specified game state.;
    /// </summary>
    /// <typeparam name="TAction">Type of the game action.</typeparam>
    /// <typeparam name="TGameState">Type of the game state.</typeparam>
    public interface IGameActionsGenerator<out TAction, in TGameState>
    {
        /// <summary>
        /// Generates action based on the current game state.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        IReadOnlyList<TAction> Generate(TGameState currentGameState);
    }
}