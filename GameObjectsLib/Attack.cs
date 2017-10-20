namespace GameObjectsLib
{
    using GameMap;
    using ProtoBuf;

    /// <summary>
    ///     Represents one attack in the game round.
    /// </summary>
    [ProtoContract]
    public struct Attack
    {
        /// <summary>
        ///     Represents attacking region.
        /// </summary>
        [ProtoMember(1, AsReference = true)]
        public Region Attacker { get; }

        /// <summary>
        ///     Attacking army, must be lower or equal than Attacker region army.
        /// </summary>
        [ProtoMember(2)]
        public int AttackingArmy { get; }

        /// <summary>
        ///     Defending region.
        /// </summary>
        [ProtoMember(3, AsReference = true)]
        public Region Defender { get; }

        public Attack(Region attacker, int attackingArmy, Region defender)
        {
            //if (attacker == null || attackingArmy == 0 || defender == null)
            //    throw new ArgumentException();

            Attacker = attacker;
            AttackingArmy = attackingArmy;
            Defender = defender;
        }
    }
}