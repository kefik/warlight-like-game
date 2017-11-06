namespace GameObjectsLib.GameRecording
{
    using GameMap;
    using ProtoBuf;

    /// <summary>
    /// Represents one deploy of units.
    /// </summary>
    [ProtoContract]
    public class Deployment
    {
        [ProtoMember(1, AsReference = true)]
        public Region Region { get; }

        [ProtoMember(2)]
        public int Army { get; }

        // ReSharper disable once UnusedMember.Local
        private Deployment() { }

        public Deployment(Region region, int army)
        {
            Region = region;
            Army = army;
        }
    }
}