namespace GameObjectsLib.GameRecording
{
    using GameMap;
    using Players;
    using ProtoBuf;

    /// <summary>
    ///     Represents one attack in the game round.
    /// </summary>
    [ProtoContract]
    public class Attack : IAction
    {
        /// <summary>
        /// Represents attacking player in this attack.
        /// </summary>
        [ProtoMember(1, AsReference = true)]
        public Player AttackingPlayer { get; internal set; }

        public Player ActionInvoker
        {
            get { return AttackingPlayer; }
        }

        /// <summary>
        ///     Represents attacking region.
        /// </summary>
        [ProtoMember(2, AsReference = true)]
        public Region Attacker { get; internal set; }

        /// <summary>
        ///     Attacking army, must be lower or equal than Attacker region army.
        /// </summary>
        [ProtoMember(3)]
        public int AttackingArmy { get; internal set; }

        /// <summary>
        ///     Defending region.
        /// </summary>
        [ProtoMember(4, AsReference = true)]
        public Region Defender { get; internal set; }

        /// <summary>
        /// Represents how the map changed after this attack.
        /// Precisely new state of the <see cref="Defender"/>
        /// and <seealso cref="Attacker"/>.
        /// It's only set in <see cref="LinearizedGameRound"/>
        /// and only after the round evaluation finished.
        /// If the round was already evaluated but <see cref="PostAttackMapChange"/>
        /// is still null it means, that the attack did not happen (attackin
        /// region changed owner).
        /// </summary>
        [ProtoMember(5, AsReference = true)]
        public PostAttackMapChange PostAttackMapChange { get; set; }

        // ReSharper disable once UnusedMember.Local
        private Attack()
        {
            
        }

        public Attack(Player attackingPlayer, Region attacker, int attackingArmy, Region defender)
        {
            //if (attacker == null || attackingArmy == 0 || defender == null)
            //    throw new ArgumentException();

            AttackingPlayer = attackingPlayer;
            Attacker = attacker;
            AttackingArmy = attackingArmy;
            Defender = defender;
        }

        public bool DoesConcernRegion(Region region)
        {
            return Attacker == region || Defender == region;
        }

        public bool IsCloseOrRelatedTo(Player player)
        {
            return player.IsRegionMine(Attacker)
                || player.IsRegionMine(Defender)
                || Attacker.IsNeighbourOf(player)
                || Defender.IsNeighbourOf(player);
        }

        public override string ToString()
        {
            string value = $"Attacker: {AttackingPlayer?.Name}, " +
                           $"{Attacker?.Name} -> " +
                           $"{Defender?.Name}, " +
                           $"{AttackingArmy}";
            return value;
        }
    }
}