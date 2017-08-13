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
        public int Id { get; set; }

        public int OpenedSlotsNumber { get; set; }

        public string MapName { get; set; }

        public int AIPlayersCount { get; set; }

        public int HumanPlayersCount { get; set; }

        public ICollection<User> SignedUsers { get; set; }
        
        public byte[] SerializedGame { get; set; }
    }
}
