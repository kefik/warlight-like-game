namespace Server.Entities
{
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using GameObjectsLib.Game;

    public class WarlightDbContext : DbContext, IGameSaver<OpenedGame>, IGameSaverAsync<OpenedGame>
    {
        public WarlightDbContext()
            : base("name=WarlightDbContext")
        {
        }

        // ReSharper disable once EmptyConstructor
        static WarlightDbContext()
        {
            //Database.SetInitializer(new WarlightDbDropCreateIfModelChangesInitializer());
        }

        public virtual DbSet<StartedGame> StartedGames { get; set; }
        public virtual DbSet<OpenedGame> OpenedGames { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<MapInfo> Maps { get; set; }
        public virtual DbSet<LastRound> LastRounds { get; set; }
        public virtual DbSet<LastTurn> LastTurns { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region User relations

            // m: n Games -Users
            modelBuilder.Entity<User>()
                .HasMany(user => user.StartedGames)
                .WithMany(game => game.PlayingUsers)
                .Map(manyToManyAssociationMappingConfiguration =>
                {
                    manyToManyAssociationMappingConfiguration.MapLeftKey($"{nameof(User)}{nameof(User.Id)}");
                    manyToManyAssociationMappingConfiguration.MapRightKey(
                        $"{nameof(StartedGame)}{nameof(StartedGame.Id)}");
                    manyToManyAssociationMappingConfiguration.ToTable("StartedGamesUsers");
                });

            //// m : n OpenedGames - Users
            modelBuilder.Entity<User>()
                .HasMany(user => user.OpenedGames)
                .WithMany(openedGame => openedGame.SignedUsers)
                .Map(x =>
                {
                    x.MapLeftKey($"{nameof(User)}{nameof(User.Id)}");
                    x.MapRightKey($"{nameof(OpenedGame)}{nameof(OpenedGame.Id)}");
                    x.ToTable("OpenedGamesUsers");
                });

            #endregion

            #region LastRound relations

            modelBuilder.Entity<LastRound>()
                .HasMany(x => x.LastTurns)
                .WithRequired(x => x.LastRound)
                .WillCascadeOnDelete(true);

            #endregion
        }

        public OpenedGame GetMatchingOpenedGame(int id)
        {
            return (from openedGame in OpenedGames
                    where openedGame.Id == id
                    select openedGame).AsEnumerable().FirstOrDefault();
        }

        public User GetMatchingUser(string login)
        {
            return (from user in Users
                    where user.Name == login
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
            gameMetaInfo.SetGame(stream);
            OpenedGames.Add(gameMetaInfo);
            SaveChanges();
        }

        public async Task SaveGameAsync(OpenedGame gameMetaInfo, Stream stream)
        {
            await gameMetaInfo.SetGameAsync(stream);
            OpenedGames.Add(gameMetaInfo);
            await SaveChangesAsync();
        }
    }
}
