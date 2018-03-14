namespace GameObjectsLib.GameRecording
{
    using System.Linq;
    using GameMap;
    using Players;
    using ProtoBuf;

    [ProtoContract]
    public class Seize : IAction
    {
        [ProtoMember(1, AsReference = true)]
        public Player SeizingPlayer { get; internal set; }

        [ProtoMember(2, AsReference = true)]
        public Region Region { get; internal set; }

        public bool RanSuccessfully
        {
            get { return true; }
        }

        public Player ActionInvoker
        {
            get { return SeizingPlayer; }
        }

        // ReSharper disable once UnusedMember.Local
        private Seize() { }

        public Seize(Player player, Region region)
        {
            SeizingPlayer = player;
            Region = region;
        }
        
        public bool DoesConcernRegion(Region region)
        {
            return Region == region;
        }

        public bool IsCloseOrRelatedTo(Player player)
        {
            return SeizingPlayer == player
                || Region.IsNeighbourOf(player);
        }

        public override string ToString()
        {
            return $"{SeizingPlayer?.Name} {Region?.Name}";
        }
    }
}