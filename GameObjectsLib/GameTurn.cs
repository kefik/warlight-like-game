namespace GameObjectsLib
{
    using System.Collections.Generic;
    using System.Linq;
    using GameMap;
    using Players;
    using ProtoBuf;

    [ProtoContract]
    public class GameTurn : Turn
    {
        /// <summary>
        ///     Represents deploying phase of the game round.
        /// </summary>
        [ProtoMember(2)]
        public Deploying Deploying { get; set; }

        /// <summary>
        ///     Represents attacking phase of the game round.
        /// </summary>
        [ProtoMember(3)]
        public Attacking Attacking { get; set; }

        public GameTurn(Deploying deploying, Attacking attacking, Player playerOnTurn) : base(playerOnTurn)
        {
            Deploying = deploying;
            Attacking = attacking;
        }

        public GameTurn(Player playerOnTurn) : base(playerOnTurn)
        {
            Deploying = new Deploying(new List<Deployment>());
            Attacking = new Attacking(new List<Attack>());
        }

        public override void Reset()
        {
            Deploying.ResetDeploying();
            Attacking.ResetAttacking();
        }

        /// <summary>
        /// Calculates real region army including deploying and attacking changes.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public int GetRegionArmy(Region region)
        {
            var deployRegionEnumerable = (from tuple in Deploying.ArmiesDeployed
                                          where tuple.Region == region
                                          select tuple.Army).ToList();
            var attackRegionEnumerable = (from attack in Attacking.Attacks
                                          where attack.Attacker == region
                                          select attack.AttackingArmy).ToList();
            // nothing was deployed in this region
            if (!deployRegionEnumerable.Any())
            {
                return region.Army - attackRegionEnumerable.Sum();
            }
            // nothing was attacked with
            if (!attackRegionEnumerable.Any())
            {
                return deployRegionEnumerable.Sum();
            }
            return deployRegionEnumerable.Sum() - attackRegionEnumerable.Sum();
        }

        /// <summary>
        ///     Calculates units of given region that are left to attack.
        /// </summary>
        /// <param name="region">Given region.</param>
        /// <returns></returns>
        public int GetUnitsLeftToAttack(Region region)
        {
            return GetRegionArmy(region) - Region.MinimumArmy;
        }
    }
}