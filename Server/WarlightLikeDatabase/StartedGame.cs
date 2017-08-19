namespace Server.WarlightLikeDatabase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class StartedGame : GameEntity
    {
        [Required]
        [MaxLength(20480)]
        public virtual byte[] SerializedRounds { get; set; }

        public int? LastRoundId { get; set; }
        public LastRound LastRound { get; set; }

        [Required]
        [StringLength(50)]
        public string GameStartedDateTime { get; set; }
        
        [Required]
        public int RoundNumber { get; set; }

        [Required]
        public virtual ICollection<User> PlayingUsers { get; set; }
    }
}
