namespace Server.WarlightLikeDatabase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        public string Login { get; set; }

        [Required]
        [StringLength(50)]
        public string PasswordHash { get; set; }

        public ICollection<Game> StartedGames { get; set; }
        public ICollection<OpenedGame> UnstartedGames { get; set; }
    }
}
