namespace GameAi.Data
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Enum for types of bots one can implement.
    /// </summary>
    public enum GameBotType
    {
        /// <summary>
        /// Bot will be randomly chosen.
        /// </summary>
        [Display(Name = "Random bot")]
        Random,
        /// <summary>
        /// Bot using MCTS for finding the best move.
        /// </summary>
        [Display(Name = "MCTS bot")]
        MonteCarloTreeSearchBot
    }
}