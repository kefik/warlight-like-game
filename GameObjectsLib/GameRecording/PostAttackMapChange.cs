namespace GameObjectsLib.GameRecording
{
    using GameMap;
    using Players;
    using ProtoBuf;

    /// <summary>
    /// Represents changes that happened to this map after <see cref="Attack"/>
    /// happened.
    /// </summary>
    [ProtoContract]
    public class PostAttackMapChange
    {
        /// <summary>
        /// Army of the attacking region
        /// after the <see cref="Attack"/>.
        /// </summary>
        [ProtoMember(1)]
        public int AttackingRegionArmy { get; set; }

        /// <summary>
        /// Owner of the given defending region (possibly new).
        /// </summary>
        [ProtoMember(2, AsReference = true)]
        public Player DefendingRegionOwner { get; set; }

        /// <summary>
        /// Army of the defending region after the <see cref="Attack"/>.
        /// </summary>
        [ProtoMember(3)]
        public int DefendingRegionArmy { get; set; }
    }
}