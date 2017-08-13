namespace Server.WarlightLikeDatabase
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LastRound
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(6144)]
        public byte[] SerializedAiTurns { get; set; }

        public ICollection<LastTurn> LastTurns { get; set; }
    }
}
