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
    }
}