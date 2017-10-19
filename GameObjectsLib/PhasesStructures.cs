namespace GameObjectsLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameMap;
    using ProtoBuf;

    /// <summary>
    ///     Represents deploying phase of the game.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public struct Deploying
    {
        /// <summary>
        ///     Represents armies deployed in the deploying phase in given regions.
        ///     Int represents armies that will be occuppying this region after this stage.
        /// </summary>
        public List<Deploy> ArmiesDeployed { get; }

        public Deploying(List<Deploy> armiesDeployed)
        {
            ArmiesDeployed = armiesDeployed;
        }

        /// <summary>
        ///     Calculates how many units can given player deploy to fulfill his maximum.
        /// </summary>
        /// <param name="player">Given player.</param>
        /// <returns>Units left to deploy for given player.</returns>
        public int GetUnitsLeftToDeploy(Player player)
        {
            int income = player.GetIncome();
            int alreadyDeployed = (from deploy in ArmiesDeployed
                                   where deploy.Region.Owner == player
                                   select deploy.Army - deploy.Army).Sum();
            return income - alreadyDeployed;
        }
    }

    /// <summary>
    /// Represents one deploy of units.
    /// </summary>
    [ProtoContract]
    public struct Deploy
    {
        [ProtoMember(1, AsReference = true)]
        public Region Region { get; }

        [ProtoMember(2)]
        public int Army { get; }

        public Deploy(Region region, int army)
        {
            Region = region;
            Army = army;
        }
    }

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
    }

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
