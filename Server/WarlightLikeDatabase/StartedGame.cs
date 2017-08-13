namespace Server.WarlightLikeDatabase
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class StartedGame
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(20480)]
        public virtual byte[] SerializedGame { get; set; }

        [Required]
        [MaxLength(20480)]
        public virtual byte[] SerializedRounds { get; set; }

        public int? LastRoundId { get; set; }
        public LastRound LastRound { get; set; }

        [Required]
        public int RoundNumber { get; set; }

        public virtual ICollection<User> PlayingUsers { get; set; }
    }
}
