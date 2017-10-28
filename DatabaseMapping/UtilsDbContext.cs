using System;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using GameObjectsLib;
using GameObjectsLib.Game;
using SQLiteProviderFactory = System.Data.SQLite.EF6.SQLiteProviderFactory;

namespace WinformsUI
{
    using System.Threading.Tasks;

    public class UtilsDbContext : DbContext,
        IGameSaver<Game>,
        IGameLoader<SingleplayerSavedGameInfo>,
        IGameLoader<HotseatSavedGameInfo>
    {
        public UtilsDbContext() :
            base(new SQLiteConnection()
            {
                ConnectionString = new SQLiteConnectionStringBuilder()
                {
                    DataSource = @"Utils.db",
                    ForeignKeys = true
                }.ConnectionString
            }, true)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public void SaveGame(Game game, Stream stream)
        {
            switch (game.GameType)
            {
                case GameType.SinglePlayer:
                    {
                        var savedGames = SingleplayerSavedGameInfos;
                        string path = string.Format($"SavedGames/Singleplayer/{game.Id}.sav");
                        
                        var save = savedGames.FirstOrDefault(x => x.Id == game.Id);
                        // game hasn't been saved yet
                        if (save == null)
                        {
                            savedGames.Add(new SingleplayerSavedGameInfo()
                            {
                                AiNumber = game.Players.Count - 1,
                                MapName = game.Map.Name,
                                SavedGameDate = DateTime.Now.ToString(),
                                Path = path
                            });
                        }
                        else save.SavedGameDate = DateTime.Now.ToString();
                        
                        // write the game into file
                        using (FileStream fs = new FileStream(path, FileMode.Create))
                        {
                            stream.CopyTo(fs);
                        }
                        SaveChanges();
                        break;
                    }
                case GameType.MultiplayerHotseat:
                    {
                        var savedGames = HotseatSavedGameInfos;
                        string path = string.Format($"SavedGames/Hotseat/{game.Id}.sav");
                        
                        var save = savedGames.FirstOrDefault(x => x.Id == game.Id);

                        if (save == null)
                        {
                            int aiPlayerNumber = (from player in game.Players
                                                  where player.GetType() == typeof(AiPlayer)
                                                  select player).Count();
                            int humanPlayersNumber = game.Players.Count - aiPlayerNumber;

                            savedGames.Add(new HotseatSavedGameInfo()
                            {
                                Id = game.Id,
                                AiNumber = aiPlayerNumber,
                                HumanNumber = humanPlayersNumber,
                                MapName = game.Map.Name,
                                SavedGameDate = DateTime.Now.ToString(),
                                Path = path
                            });
                        }
                        else save.SavedGameDate = DateTime.Now.ToString();
                        
                        

                        using (var fs = new FileStream(path, FileMode.Create))
                        {
                            stream.CopyTo(fs);
                        }

                        SaveChanges();

                        break;
                    }
                case GameType.MultiplayerNetwork:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public Stream LoadGame(SingleplayerSavedGameInfo info)
        {
            var savedGameInfo = SingleplayerSavedGameInfos.ToList().First(x => x.Id == info.Id);
            FileStream fs = new FileStream(savedGameInfo.Path, FileMode.Open);

            return fs;
        }

        public Stream LoadGame(HotseatSavedGameInfo info)
        {
            var savedGameInfo = HotseatSavedGameInfos.First(x => x.Id == info.Id);
            FileStream fs = new FileStream(savedGameInfo.Path, FileMode.Open);

            return fs;
        }

        public void Remove(SingleplayerSavedGameInfo savedGameInfo)
        {
            using (DbContextTransaction transaction = Database.BeginTransaction())
            {
                try
                {
                    var objectToBeRemoved = SingleplayerSavedGameInfos.First(x => x.Id == savedGameInfo.Id);

                    string path = objectToBeRemoved.Path;

                    SingleplayerSavedGameInfos.Remove(objectToBeRemoved);
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
            using (DbContextTransaction transaction = Database.BeginTransaction())
            {
                try
                {
                    var objectToBeRemoved = HotseatSavedGameInfos.First(x => x.Id == savedGameInfo.Id);

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
        
        public virtual DbSet<MapInfo> Maps { get; set; }
        public virtual DbSet<SingleplayerSavedGameInfo> SingleplayerSavedGameInfos { get; set; }
        public virtual DbSet<HotseatSavedGameInfo> HotseatSavedGameInfos { get; set; }
    }

    public class SQLiteConfiguration : DbConfiguration
    {
        public SQLiteConfiguration()
        {
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", System.Data.SQLite.EF6.SQLiteProviderFactory.Instance);
            SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }
    }
}
