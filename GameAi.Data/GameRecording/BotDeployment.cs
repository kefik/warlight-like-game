namespace GameAi.Data.GameRecording
{
    public struct BotDeployment
    {
        public int RegionId { get; set; }
        public int Army { get; set; }
        public int DeployingPlayerId { get; set; }

        public BotDeployment(int regionId, int army, int deployingPlayerId)
        {
            RegionId = regionId;
            Army = army;
            DeployingPlayerId = deployingPlayerId;
        }

        public override string ToString()
        {
            return $"{DeployingPlayerId}, {RegionId}, {Army}";
        }
    }
}