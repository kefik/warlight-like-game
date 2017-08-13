namespace Server.WarlightLikeDatabase
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using GameObjectsLib.Game;
    using GameObjectsLib.NetworkCommObjects;

    public partial class WarlightDbContext : DbContext, IGameSaver<OpenedGame>, IGameSaverAsync<OpenedGame>
    {
        public WarlightDbContext()
            : base("name=WarlightDbContext")
        {
        }

        static WarlightDbContext()
        {
            Database.SetInitializer(new WarlightDbDropCreateIfModelChangesInitializer());
        }

        public virtual DbSet<StartedGame> Games { get; set; }
        public virtual DbSet<OpenedGame> OpenedGames { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<MapInfo> Maps { get; set; }
        public virtual DbSet<LastRound> LastRounds { get; set; }
        public virtual DbSet<LastTurn> LastTurns { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region UserRelations
            // m : n Games - Users
            modelBuilder.Entity<User>()
                .HasMany(user => user.StartedGames)
                .WithMany(game => game.PlayingUsers)
                .Map(manyToManyAssociationMappingConfiguration =>
                {
                    manyToManyAssociationMappingConfiguration.MapLeftKey(nameof(User.Id));
                    manyToManyAssociationMappingConfiguration.MapRightKey(nameof(Game.Id));
                    manyToManyAssociationMappingConfiguration.ToTable("GamesUsers");
                });

            // m : n OpenedGames - Users
            modelBuilder.Entity<User>()
                .HasMany(user => user.OpenedGames)
                .WithMany(openedGame => openedGame.SignedUsers)
                .Map(x =>
                {
                    x.MapLeftKey(nameof(User.Id));
                    x.MapRightKey(nameof(OpenedGame.Id));
                    x.ToTable("OpenedGamesUsers");
                });
            #endregion

        }

        public User GetMatchingUser(string login)
        {
            return (from user in Users
                    where user.Login == login
                    select user).AsEnumerable().FirstOrDefault();
        }

        public MapInfo GetMatchingMap(string mapName)
        {
            return (from info in Maps
                    where info.Name == mapName
                    select info).AsEnumerable().FirstOrDefault();
        }

        public int GetMaxOpenedGameId()
        {
            return OpenedGames.Any() == false ? 0 : OpenedGames.Max(x => x.Id);
        }

        public void SaveGame(OpenedGame gameMetaInfo, Stream stream)
        {
            gameMetaInfo.SerializedGame = ((MemoryStream)stream).GetBuffer();
            OpenedGames.Add(gameMetaInfo);
            SaveChanges();
        }

        public async Task SaveGameAsync(OpenedGame gameMetaInfo, Stream stream)
        {
            gameMetaInfo.SerializedGame = ((MemoryStream)stream).GetBuffer();
            OpenedGames.Add(gameMetaInfo);
            await SaveChangesAsync();
        }
    }
}
