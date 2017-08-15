namespace Server.WarlightLikeDatabase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LastRound
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LastRoundId { get; set; }

        [MaxLength(6144)]
        [Required]
        public virtual byte[] SerializedAiTurns { get; set; }

        [Required]
        [StringLength(50)]
        public string RoundStartedDateTime { get; set; }

        public virtual ICollection<LastTurn> LastTurns { get; set; }
    }
}
