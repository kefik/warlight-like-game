namespace GameObjectsLib.GameRecording
{
    using GameMap;
    using Players;
    using ProtoBuf;

    /// <summary>
    /// Represents one deploy of units.
    /// </summary>
    [ProtoContract]
    public class Deployment : IAction
    {
        [ProtoMember(1, AsReference = true)]
        public Region Region { get; internal set; }

        [ProtoMember(2)]
        public int Army { get; internal set; }

        [ProtoMember(3, AsReference = true)]
        public Player DeployingPlayer { get; internal set; }

        public Player ActionInvoker
        {
            get { return DeployingPlayer; }
        }

        // ReSharper disable once UnusedMember.Local
        private Deployment() { }

        public Deployment(Region region, int army, Player deployingPlayer)
        {
            Region = region;
            Army = army;
            DeployingPlayer = deployingPlayer;
        }
        
        public bool DoesConcernRegion(Region region)
        {
            return Region == region;
        }

        public override string ToString()
        {
            string value = $"{DeployingPlayer?.Name} {Region?.Name} {Army}";
            return value;
        }
    }
}