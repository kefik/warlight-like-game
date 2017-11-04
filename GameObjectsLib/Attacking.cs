namespace GameObjectsLib
{
    using System.Collections.Generic;
    using System.Linq;
    using GameMap;
    using ProtoBuf;

    /// <summary>
    ///     Represents attacking phase of the game.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public struct Attacking
    {
        /// <summary>
        ///     Represents attacks that happen during attacking phase.
        /// </summary>
        public IList<Attack> Attacks { get; }

        public Attacking(List<Attack> attacks)
        {
            Attacks = attacks;
        }

        /// <summary>
        ///     Calculates units of given region that are left to attack.
        /// </summary>
        /// <param name="region">Given region.</param>
        /// <param name="deployingPhase">Deploying based on which it will be calculated.</param>
        /// <returns></returns>
        public int GetUnitsLeftToAttack(Region region, Deploying deployingPhase)
        {
            IEnumerable<int> deployRegionEnumerable = from tuple in deployingPhase.ArmiesDeployed
                                                      where tuple.Region == region
                                                      select tuple.Army;
            IEnumerable<int> attackRegionEnumerable = from attack in Attacks
                                                      where attack.Attacker == region
                                                      select attack.AttackingArmy;
            // nothing was deployed in this region
            if (!deployRegionEnumerable.Any())
            {
                // nothing was attacked with
                if (!attackRegionEnumerable.Any())
                {
                    return region.Army - Region.MinimumArmy;
                }
                return region.Army - attackRegionEnumerable.Sum() - Region.MinimumArmy;
            }
            // nothing was attacked with
            if (!attackRegionEnumerable.Any())
            {
                return deployRegionEnumerable.Sum() - Region.MinimumArmy;
            }
            return deployRegionEnumerable.Sum() - attackRegionEnumerable.Sum() - Region.MinimumArmy;
        }

        /// <summary>
        /// Adds attack to the list of attacks.
        /// </summary>
        public void AddAttack(Region attackingRegion, Region defendingRegion, int attackingArmy)
        {
            AddAttack(attackingRegion.Owner, attackingRegion, defendingRegion, attackingArmy);
        }

        public void AddAttack(Player attackingPlayer, Region attackingRegion, Region defendingRegion, int attackingArmy)
        {
            Attacks.Add(new Attack(attackingPlayer, attackingRegion, attackingArmy, defendingRegion));
        }

        /// <summary>
        /// Resets all attacks.
        /// </summary>
        public void ResetAttacking()
        {
            Attacks?.Clear();
        }
    }
}