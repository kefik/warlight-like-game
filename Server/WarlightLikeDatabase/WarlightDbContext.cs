namespace Server.WarlightLikeDatabase
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class WarlightDbContext : DbContext
    {
        public WarlightDbContext()
            : base("name=WarlightDbContext")
        {
        }

        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<OpenedGame> OpenedGames { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
