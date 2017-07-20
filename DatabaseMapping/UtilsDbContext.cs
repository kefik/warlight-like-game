using System;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using GameObjectsLib;
using GameObjectsLib.Game;
using ProtoBuf;
using SQLiteProviderFactory = System.Data.SQLite.EF6.SQLiteProviderFactory;

namespace DatabaseMapping
{
    public class UtilsDbContext : DbContext, ICanSaveGame, ICanLoadGame<SingleplayerSavedGameInfo>,
        ICanLoadGame<HotseatSavedGameInfo>
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
                        var savedGamesEnum = savedGames.AsEnumerable();

                        var lastGame = savedGamesEnum.LastOrDefault();
                        int lastGameId = 1;
                        if (lastGame != null) lastGameId = lastGame.Id + 1;

                        string path = string.Format($"SavedGames/Singleplayer/{lastGameId}.sav");

                        savedGames.Add(new SingleplayerSavedGameInfo()
                        {
                            AINumber = game.Players.Count - 1,
                            MapName = game.Map.Name,
                            SavedGameDate = DateTime.Now.ToString(),
                            Path = string.Format(path)
                        });

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

                        var savedGamesEnum = savedGames.AsEnumerable();

                        var lastGame = savedGamesEnum.LastOrDefault();
                        int lastGameId = 1;
                        if (lastGame != null) lastGameId = lastGame.Id + 1;

                        int aiPlayerNumber = (from player in game.Players
                                              where player.GetType() == typeof(AIPlayer)
                                              select player).Count();
                        int humanPlayersNumber = game.Players.Count - aiPlayerNumber;

                        savedGames.Add(new HotseatSavedGameInfo()
                        {
                            AINumber = aiPlayerNumber,
                            HumanNumber = humanPlayersNumber,
                            MapName = game.Map.Name,
                            SavedGameDate = DateTime.Now.ToString(),
                            Path = string.Format($"SavedGames/Hotseat/{lastGameId}.sav")
                        });

                        using (var fs = new FileStream($"SavedGames/Hotseat/{lastGameId}.sav", FileMode.Create))
                        {
                            Serializer.Serialize(fs, game);
                        }

                        break;
                    }
                case GameType.MultiplayerNetwork:
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
