namespace Server.WarlightLikeDatabase
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LastRound
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LastRoundId { get; set; }

        [MaxLength(6144)]
        public virtual byte[] SerializedAiTurns { get; set; }

        public virtual ICollection<LastTurn> LastTurns { get; set; }
    }
}
