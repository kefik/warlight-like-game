namespace GameObjectsLib
{
    using GameMap;
    using Players;
    using ProtoBuf;

    /// <summary>
    ///     Represents one attack in the game round.
    /// </summary>
    [ProtoContract]
    public class Attack
    {
        /// <summary>
        /// Represents attacking player in this attack.
        /// </summary>
        [ProtoMember(1, AsReference = true)]
        public Player AttackingPlayer { get; }

        /// <summary>
        ///     Represents attacking region.
        /// </summary>
        [ProtoMember(2, AsReference = true)]
        public Region Attacker { get; }

        /// <summary>
        ///     Attacking army, must be lower or equal than Attacker region army.
        /// </summary>
        [ProtoMember(3)]
        public int AttackingArmy { get; }

        /// <summary>
        ///     Defending region.
        /// </summary>
        [ProtoMember(4, AsReference = true)]
        public Region Defender { get; }

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
    }
}