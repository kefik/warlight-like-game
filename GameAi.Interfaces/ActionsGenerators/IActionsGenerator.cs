namespace GameAi.Interfaces.ActionsGenerators
{
    using System.Collections.Generic;
    using Data.GameRecording;

    /// <summary>
    /// Class inheriting from this can generate
    /// game action for specified game state.;
    /// </summary>
    /// <typeparam name="TAction">Type of the game action.</typeparam>
    /// <typeparam name="TGameState">Type of the game state.</typeparam>
    public interface IActionsGenerator<out TAction, in TGameState>
        where TAction : BotTurn
    {
        /// <summary>
        /// Generates action based on the current game state.
        /// </summary>
        /// <param name="currentGameState"></param>
        /// <returns></returns>
        IReadOnlyList<TAction> Generate(TGameState currentGameState);
    }
}