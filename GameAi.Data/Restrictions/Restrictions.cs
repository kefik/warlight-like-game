namespace GameAi.Data.Restrictions
{
    using System.Collections.Generic;
    using Interfaces;
    using ProtoBuf;

    /// <summary>
    /// Represents restrictions applied for given instance of the game.
    /// </summary>
    [ProtoContract]
    public class Restrictions
    {
        /// <summary>
        /// Restrictions for the game beginning part.
        /// </summary>
        [ProtoMember(1)]
        public ICollection<GameBeginningRestriction> GameBeginningRestrictions { get; set; }
    }
}