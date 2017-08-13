namespace Server.WarlightLikeDatabase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Data.Entity.Spatial;

    public class OpenedGame
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OpenedGameId { get; set; }

        [Required]
        public int OpenedSlotsNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string MapName { get; set; }

        [Required]
        public int AiPlayersCount { get; set; }

        [Required]
        public int HumanPlayersCount { get; set; }

        [Required]
        public virtual ICollection<User> SignedUsers { get; set; }
        
        [Required]
        [MaxLength(20480)]
        public virtual byte[] SerializedGame { get; set; }
    }
}
