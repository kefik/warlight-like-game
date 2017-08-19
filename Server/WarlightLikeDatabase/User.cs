namespace Server.WarlightLikeDatabase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class User : NamedEntity
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        [Index(IsUnique = true)]
        public override string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        public virtual ICollection<StartedGame> StartedGames { get; set; }
        public virtual ICollection<OpenedGame> OpenedGames { get; set; }
    }
}
