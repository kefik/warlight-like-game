namespace InterFormatCommunication.Restrictions
{
    using System.Collections.Generic;
    using GameAi.Interfaces;

    public class Restrictions
    {
        public ICollection<IGameBeginningRestriction> GameBeginningRestrictions { get; set; }
        public ICollection<IGameRestriction> GameRestrictions { get; set; }
    }
}