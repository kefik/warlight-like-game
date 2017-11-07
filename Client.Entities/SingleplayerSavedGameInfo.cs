namespace Client.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Configuration;
    
    public class SingleplayerSavedGameInfo : GameEntity
    {
        protected override string SavedGamesStoragePath { get; } =
            ConfigurationManager.AppSettings["SingleplayerSavedGamesStoragePath"];

        private SingleplayerSavedGameInfo() { }

        public SingleplayerSavedGameInfo(byte[] data)
        {
            Data = data;
        }

        public override string ToString()
        {
            return string.Format($"Ai: {AiNumber}; Map: {MapName}, Saved: {SavedGameDate}");
        }
    }
}
