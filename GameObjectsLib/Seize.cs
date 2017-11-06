namespace GameObjectsLib
{
    using GameMap;
    using ProtoBuf;

    [ProtoContract]
    public struct Seize
    {
        [ProtoMember(1, AsReference = true)]
        public Player.Player SeizingPlayer { get; }

        [ProtoMember(2, AsReference = true)]
        public Region Region { get; }

        public Seize(Player.Player player, Region region)
        {
            SeizingPlayer = player;
            Region = region;
        }
    }
}