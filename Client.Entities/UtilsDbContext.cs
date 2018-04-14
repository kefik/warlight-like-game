namespace Client.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Data.SQLite;
    using System.Data.SQLite.EF6;
    using System.IO;
    using System.Linq;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.Players;

    public class UtilsDbContext
        : DbContext, IGameSaver<Game>,
            IGameLoader<SingleplayerSavedGameInfo>,
            IGameLoader<HotseatSavedGameInfo>,
            IGameLoader<SimulationRecord>
    {
        public virtual DbSet<MapInfo> Maps { get; set; }

        public virtual DbSet<SingleplayerSavedGameInfo>
            SingleplayerSavedGameInfos { get; set; }

        public virtual DbSet<HotseatSavedGameInfo>
            HotseatSavedGameInfos { get; set; }

        public virtual DbSet<SimulationRecord> SimulationRecords
        {
            get;
            set;
        }

        public UtilsDbContext() : base(
            new SQLiteConnection()
            {
                ConnectionString =
                    new SQLiteConnectionStringBuilder()
                    {
                        DataSource = @"Utils.db",
                        ForeignKeys = true
                    }.ConnectionString
            }, true)
        {
        }

        protected override void OnModelCreating(
            DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<SingleplayerSavedGameInfo>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable(nameof(SingleplayerSavedGameInfo) + "s");
            });
            modelBuilder.Entity<HotseatSavedGameInfo>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable(nameof(HotseatSavedGameInfo) + "s");
            });
            modelBuilder.Entity<SimulationRecord>().Map(m =>
            {
                m.ToTable(nameof(SimulationRecord) + "s");
                m.MapInheritedProperties();
            });
            modelBuilder.Entity<MapInfo>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable(nameof(MapInfo) + "s");
            });
            modelBuilder.Configurations.Add(
                new GameEntity.GameEntityMapper());

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            // calls deleted method on each entry deleted of type GameEntity
            var deletedEntries = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Deleted)
                .Select(x => x.Entity).OfType<IDeleted>();
            foreach (var entry in deletedEntries)
            {
                entry.Deleted();
            }

            // calls inserted method on each entry added of type GameEntity
            var addedEntries = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added)
                .Select(x => x.Entity).OfType<IInserted>();
            foreach (var entry in addedEntries)
            {
                entry.Inserted();
            }

            return base.SaveChanges();
        }

        public void SaveGame(Game game)
        {
            switch (game.GameType)
            {
                case GameType.SinglePlayer:
                {
                    var savedGames = SingleplayerSavedGameInfos;
                    string name = $"{game.Id}.sav";

                    var save =
                        savedGames.FirstOrDefault(
                            x => x.Id == game.Id);
                    var saveInfo =
                        new SingleplayerSavedGameInfo(game
                            .GetBytes())
                        {
                            AiNumber = game.Players.Count - 1,
                            MapName = game.Map.Name,
                            SavedGameDate = DateTime.Now,
                            FileName = name
                        };
                    // game hasn't been saved yet

                    if (save != null)
                    {
                        savedGames.Remove(save);
                    }
                    savedGames.Add(saveInfo);

                    SaveChanges();
                    break;
                }
                case GameType.MultiplayerHotseat:
                {
                    var savedGames = HotseatSavedGameInfos;
                    string name = $"{game.Id}.sav";

                    var save =
                        savedGames.FirstOrDefault(
                            x => x.Id == game.Id);

                    var saveInfo =
                        new HotseatSavedGameInfo(game.GetBytes())
                        {
                            Id = game.Id,
                            MapName = game.Map.Name,
                            SavedGameDate = DateTime.Now,
                            FileName = name
                        };
                    if (save != null)
                    {
                        savedGames.Remove(save);
                    }
                    int aiPlayerNumber =
                    (from player in game.Players
                     where player.GetType() == typeof(AiPlayer)
                     select player).Count();
                    int humanPlayersNumber =
                        game.Players.Count - aiPlayerNumber;

                    saveInfo.AiNumber = aiPlayerNumber;
                    saveInfo.HumanNumber = humanPlayersNumber;
                    savedGames.Add(saveInfo);

                    SaveChanges();

                    break;
                }
                case GameType.MultiplayerNetwork:
                    throw new NotImplementedException();
                    break;
                case GameType.Simulator:
                {
                    var savedGames = SimulationRecords;
                    string name = $"{game.Id}.sav";

                    var save =
                        savedGames.FirstOrDefault(
                            x => x.Id == game.Id);
                    var saveInfo =
                        new SimulationRecord(game
                            .GetBytes())
                        {
                            Id = game.Id,
                            AiNumber = game.Players.Count,
                            MapName = game.Map.Name,
                            SavedGameDate = DateTime.Now,
                            FileName = name
                        };
                    // game hasn't been saved yet

                    if (save != null)
                    {
                        savedGames.Remove(save);
                    }
                    savedGames.Add(saveInfo);

                    SaveChanges();
                    break;
                }
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public byte[] LoadGame(SingleplayerSavedGameInfo info)
        {
            var savedGameInfo =
                SingleplayerSavedGameInfos
                    .First(x => x.Id == info.Id);

            return savedGameInfo.GetFileBytes();
        }

        public byte[] LoadGame(HotseatSavedGameInfo info)
        {
            var savedGameInfo =
                HotseatSavedGameInfos.First(x => x.Id == info.Id);

            return savedGameInfo.GetFileBytes();
        }

        public byte[] LoadGame(SimulationRecord simulationRecord)
        {
            var savedGameInfo =
                SimulationRecords.First(x => x.Id == simulationRecord.Id);

            return savedGameInfo.GetFileBytes();
        }

        public void Remove(SingleplayerSavedGameInfo savedGameInfo)
        {
            using (DbContextTransaction transaction =
                Database.BeginTransaction())
            {
                try
                {
                    var objectToBeRemoved =
                        SingleplayerSavedGameInfos.First(
                            x => x.Id == savedGameInfo.Id);

                    string path = objectToBeRemoved.Path;

                    SingleplayerSavedGameInfos.Remove(
                        objectToBeRemoved);
                    File.Delete(path);

                    SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Remove(HotseatSavedGameInfo savedGameInfo)
        {
            using (DbContextTransaction transaction =
                Database.BeginTransaction())
            {
                try
                {
                    var objectToBeRemoved =
                        HotseatSavedGameInfos.First(
                            x => x.Id == savedGameInfo.Id);

                    string path = objectToBeRemoved.Path;

                    HotseatSavedGameInfos.Remove(objectToBeRemoved);

                    File.Delete(path);

                    SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Remove(SimulationRecord simulationRecord)
        {
            using (DbContextTransaction transaction =
                Database.BeginTransaction())
            {
                try
                {
                    var objectToBeRemoved =
                        SimulationRecords.First(
                            x => x.Id == simulationRecord.Id);

                    string path = objectToBeRemoved.Path;

                    SimulationRecords.Remove(objectToBeRemoved);

                    File.Delete(path);

                    SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public class SQLiteConfiguration : DbConfiguration
    {
        public SQLiteConfiguration()
        {
            SetProviderFactory("System.Data.SQLite",
                SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6",
                System.Data.SQLite.EF6.SQLiteProviderFactory
                    .Instance);
            SetProviderServices("System.Data.SQLite",
                (DbProviderServices) SQLiteProviderFactory.Instance
                    .GetService(typeof(DbProviderServices)));
        }
    }
}
