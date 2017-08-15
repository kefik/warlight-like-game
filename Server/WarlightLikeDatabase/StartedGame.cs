namespace Server.WarlightLikeDatabase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class StartedGame
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StartedGameId { get; set; }

        [Required]
        [MaxLength(20480)]
        public virtual byte[] SerializedGame { get; set; }

        [Required]
        [MaxLength(20480)]
        public virtual byte[] SerializedRounds { get; set; }

        public int? LastRoundId { get; set; }
        public LastRound LastRound { get; set; }

        [Required]
        [StringLength(50)]
        public string GameStartedDateTime { get; set; }

        [Required]
        [StringLength(50)]
        public string MapName { get; set; }

        [Required]
        public int AiPlayersCount { get; set; }

        [Required]
        public int HumanPlayersCount { get; set; }

        [Required]
        public int RoundNumber { get; set; }

        public virtual ICollection<User> PlayingUsers { get; set; }
    }
}
