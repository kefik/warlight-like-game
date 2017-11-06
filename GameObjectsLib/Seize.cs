namespace GameObjectsLib
{
    using GameMap;
    using Players;
    using ProtoBuf;

    [ProtoContract]
    public class Seize
    {
        [ProtoMember(1, AsReference = true)]
        public Player SeizingPlayer { get; }

        [ProtoMember(2, AsReference = true)]
        public Region Region { get; }

        // ReSharper disable once UnusedMember.Local
        private Seize() { }

        public Seize(Player player, Region region)
        {
            SeizingPlayer = player;
            Region = region;
        }
    }
}