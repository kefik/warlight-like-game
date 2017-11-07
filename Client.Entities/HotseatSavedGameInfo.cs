namespace Client.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Configuration;
    
    public class HotseatSavedGameInfo : GameEntity
    {
        protected override string SavedGamesStoragePath { get; } =
            ConfigurationManager.AppSettings["HotseatSavedGamesStoragePath"];

        public virtual int HumanNumber { get; set; }

        private HotseatSavedGameInfo() { }

        public HotseatSavedGameInfo(byte[] data)
        {
            Data = data;
        }

        public override string ToString()
        {
            return string.Format($"Human: {HumanNumber}, Ai: {AiNumber}; Map: {MapName}, Saved: {SavedGameDate}");
        }
    }
}
