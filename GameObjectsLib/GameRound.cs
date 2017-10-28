namespace GameObjectsLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameMap;
    using ProtoBuf;

    /// <summary>
    ///     Instance of this place represents a game round, which consists of each
    ///     players round. Round identifies what has happened between two game states.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public sealed class GameRound : Round
    {
        /// <summary>
        ///     Represents deploying phase of the game round.
        /// </summary>
        public Deploying Deploying { get; set; }

        /// <summary>
        ///     Represents attacking phase of the game round.
        /// </summary>
        public Attacking Attacking { get; set; }

        public GameRound(Deploying deploying, Attacking attacking)
        {
            Deploying = deploying;
            Attacking = attacking;
        }

        public GameRound()
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
        ///     Processes the round from different players, ordering them correctly.
        ///     Rounds in parameters must always be one from the same player.
        /// </summary>
        /// <param name="rounds">Rounds, each from one certain player.</param>
        /// <returns>Round consisting of round in parameter, ordered differently.</returns>
        public static GameRound Process(IList<GameRound> rounds)
        {
            //if (rounds == null || rounds.Count == 0) return null;

            Deploying deploying = new Deploying(new List<Deployment>());
            Attacking attacking = new Attacking(new List<Attack>());

            int index = 0;
            while (true)
            {
                // true if anything in this round of cycle was added
                // false if nothing happened => everything was solved => can break out
                bool didSomething = false;
                foreach (GameRound round in rounds)
                {
                    if (round.Deploying.ArmiesDeployed.Count > index)
                    {
                        Deployment armyDeployed = round.Deploying.ArmiesDeployed[index];
                        deploying.ArmiesDeployed.Add(armyDeployed);
                        didSomething = true;
                    }

                    if (round.Attacking.Attacks.Count > index)
                    {
                        Attack attack = round.Attacking.Attacks[index];
                        attacking.Attacks.Add(attack);
                        didSomething = true;
                    }
                }

                if (!didSomething)
                {
                    break;
                }

                index++;
            }

            return new GameRound(deploying, attacking);
        }

        //class RoundValidator
        //{
        //    readonly Round round;
        //    public RoundValidator(Round round)
        //    {
        //        this.round = round;
        //    }

        //    /// <summary>
        //    /// Checks whether the round is played by single player.
        //    /// </summary>
        //    /// <returns>True if it is.</returns>
        //    public bool IsFromSamePlayer()
        //    {
        //        // TODO: debug
        //        // deploying
        //        {
        //            var firstOwner = (from tuple in round.Deploying.ArmiesDeployed
        //                              select tuple.Item1.Owner).FirstOrDefault();
        //            bool anyDifferentThanTheFirstOwner = (from tuple in round.Deploying.ArmiesDeployed
        //                                 where tuple.Item1.Owner != firstOwner
        //                                 select tuple.Item1.Owner).Any();

        //            if (anyDifferentThanTheFirstOwner) return false;
        //        }
        //        // attacking
        //        {
        //            var firstOwner = (from item in round.Attacking.Attacks
        //                              select item.Attacker?.Owner).FirstOrDefault();
        //            var anyDifferentThanTheFirstOwner = (from attack in round.Attacking.Attacks
        //                                 where attack.Attacker?.Owner != firstOwner
        //                                 select attack.Attacker.Owner).Any();

        //            if (anyDifferentThanTheFirstOwner) return false;
        //        }

        //        return true;
        //    }

        //}
        public override void Validate()
        {
            throw new NotImplementedException();
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
    }
}
