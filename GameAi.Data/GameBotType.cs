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
        /// Bot that will attack neighbours if its reasonable.
        /// </summary>
        [Display(Name = "Aggressive bot")]
        AggressiveBot,

        /// <summary>
        /// Bot that will randomly choose from good actions provided to him.
        /// </summary>
        [Display(Name = "Smart random bot")]
        SmartRandomBot,

        /// <summary>
        /// Bot using MCTS for finding the best move.
        /// </summary>
        [Display(Name = "MCTS bot")]
        MonteCarloTreeSearchBot
    }
}