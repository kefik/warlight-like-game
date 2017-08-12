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
    public class UtilsDbContext : DbContext, IGameSaver, IGameLoader<SingleplayerSavedGameInfo>,
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
                        
                        var save = (from savedGame in savedGames
                                    where savedGame.Id == game.Id
                                    select savedGame).AsEnumerable().FirstOrDefault();
                        // game hasn't been saved yet
                        if (save == null)
                        {
                            savedGames.Add(new SingleplayerSavedGameInfo()
                            {
                                AINumber = game.Players.Count - 1,
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

                        var save = (from savedGame in savedGames
                                    where savedGame.Id == game.Id
                                    select savedGame).AsEnumerable().FirstOrDefault();
                        if (save == null)
                        {
                            int aiPlayerNumber = (from player in game.Players
                                                  where player.GetType() == typeof(AIPlayer)
                                                  select player).Count();
                            int humanPlayersNumber = game.Players.Count - aiPlayerNumber;

                            savedGames.Add(new HotseatSavedGameInfo()
                            {
                                Id = game.Id,
                                AINumber = aiPlayerNumber,
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
            var savedGameInfo = (from save in SingleplayerSavedGameInfos
                                 where save.Id == info.Id
                                 select save).Single();
            FileStream fs = new FileStream(savedGameInfo.Path, FileMode.Open);

            return fs;
        }
        public Stream LoadGame(HotseatSavedGameInfo info)
        {
            var savedGameInfo = (from save in HotseatSavedGameInfos
                                 where save.Id == info.Id
                                 select save).Single();
            FileStream fs = new FileStream(savedGameInfo.Path, FileMode.Open);

            return fs;
        }

        public void Remove(SingleplayerSavedGameInfo savedGameInfo)
        {
            // TODO: rebuild to transactions
            var objectToBeRemoved = (from info in SingleplayerSavedGameInfos
                                     where info.Id == savedGameInfo.Id
                                     select info).First();

            string path = objectToBeRemoved.Path;

            SingleplayerSavedGameInfos.Remove(objectToBeRemoved);

            SaveChanges();

            File.Delete(path);
        }
        public void Remove(HotseatSavedGameInfo savedGameInfo)
        {
            // TODO: rebuild to transactions
            var objectToBeRemoved = (from info in HotseatSavedGameInfos
                                     where info.Id == savedGameInfo.Id
                                     select info).First();

            string path = objectToBeRemoved.Path;

            HotseatSavedGameInfos.Remove(objectToBeRemoved);

            SaveChanges();

            File.Delete(path);
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
