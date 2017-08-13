namespace Server.WarlightLikeDatabase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        [Index(IsClustered = false, IsUnique = true)]
        public string Login { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        public virtual ICollection<StartedGame> StartedGames { get; set; }
        public virtual ICollection<OpenedGame> OpenedGames { get; set; }
    }
}
