namespace Server.WarlightLikeDatabase
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LastTurn
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(1024)]
        [Required]
        public byte[] SerializedTurn { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int LastRoundId { get; set; }
        public LastRound LastRound { get; set; }
    }
}
