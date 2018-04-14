namespace Client.Entities
{
    using System.Configuration;

    public class SimulationRecord : GameEntity
    {
        protected override string SavedGamesStoragePath { get; } =
            ConfigurationManager.AppSettings[
                "SimulationRecordStoragePath"];

        private SimulationRecord() { }

        public SimulationRecord(byte[] data)
        {
            Data = data;
        }

        public override string ToString()
        {
            return string.Format($"Id: {Id}, Ai: {AiNumber}; Map: {MapName}, Saved: {SavedGameDate}");
        }
    }
}