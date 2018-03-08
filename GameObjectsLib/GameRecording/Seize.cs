namespace GameObjectsLib.GameRecording
{
    using GameMap;
    using Players;
    using ProtoBuf;

    [ProtoContract]
    public class Seize
    {
        [ProtoMember(1, AsReference = true)]
        public Player SeizingPlayer { get; internal set; }

        [ProtoMember(2, AsReference = true)]
        public Region Region { get; internal set; }

        // ReSharper disable once UnusedMember.Local
        private Seize() { }

        public Seize(Player player, Region region)
        {
            SeizingPlayer = player;
            Region = region;
        }
    }
}