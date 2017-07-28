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
            Database.SetInitializer(new DropCreateDatabaseAlways<WarlightDbContext>());
        }

        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<OpenedGame> OpenedGames { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // m : n Games - Users
            modelBuilder.Entity<User>()
                .HasMany(user => user.StartedGames)
                .WithMany(game => game.PlayingUsers)
                .Map(manyToManyAssociationMappingConfiguration =>
                {
                    manyToManyAssociationMappingConfiguration.MapLeftKey("UserId");
                    manyToManyAssociationMappingConfiguration.MapRightKey("GameId");
                    manyToManyAssociationMappingConfiguration.ToTable("GamesUsers");
                });

            // m : n OpenedGames - Users
            modelBuilder.Entity<User>()
                .HasMany(user => user.UnstartedGames)
                .WithMany(openedGame => openedGame.SignedUsers)
                .Map(x =>
                {
                    x.MapLeftKey("UserId");
                    x.MapRightKey("OpenedGameId");
                    x.ToTable("OpenedGamesUsers");
                });

        }
    }
}
