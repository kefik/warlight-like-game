namespace GameObjectsLib
{
    using GameMap;
    using ProtoBuf;

    /// <summary>
    /// Represents one deploy of units.
    /// </summary>
    [ProtoContract]
    public struct Deployment
    {
        [ProtoMember(1, AsReference = true)]
        public Region Region { get; }

        [ProtoMember(2)]
        public int Army { get; }

        public Deployment(Region region, int army)
        {
            Region = region;
            Army = army;
        }
    }
}