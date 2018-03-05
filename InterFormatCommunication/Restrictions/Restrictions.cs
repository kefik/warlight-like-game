namespace InterFormatCommunication.Restrictions
{
    using System.Collections.Generic;
    using GameAi.Interfaces;

    /// <summary>
    /// Represents restrictions applied for given instance of the game.
    /// </summary>
    public class Restrictions
    {
        /// <summary>
        /// Restrictions for the game beginning part.
        /// </summary>
        public ICollection<IGameBeginningRestriction> GameBeginningRestrictions { get; set; }

        /// <summary>
        /// Restrictions for other parts.
        /// </summary>
        public ICollection<IGameRestriction> GameRestrictions { get; set; }
    }
}