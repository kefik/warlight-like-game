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
    public class GameRound
    {
        /// <summary>
        ///     Represents number of the round in the game.
        ///     Round 0 represents special game round, where
        ///     players only choose their regions.
        /// </summary>
        public int Number { get; }

        /// <summary>
        ///     Represents deploying phase of the game round.
        /// </summary>
        public Deploying Deploying { get; set; }

        /// <summary>
        ///     Represents attacking phase of the game round.
        /// </summary>
        public Attacking Attacking { get; set; }

        public GameRound(int number, Deploying deploying, Attacking attacking)
        {
            Number = number;
            Deploying = deploying;
            Attacking = attacking;
        }

        public GameRound(int number)
        {
            Number = number;
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

            Deploying deploying = new Deploying(new List<Tuple<Region, int>>());
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
                        Tuple<Region, int> armyDeployed = round.Deploying.ArmiesDeployed[index];
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

            return new GameRound(rounds[0].Number, deploying, attacking);
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
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class GameBeginningRound
    {
        public List<Tuple<Player, Region>> SelectedRegions { get; } = new List<Tuple<Player, Region>>();

        public GameBeginningRound(List<Tuple<Player, Region>> list)
        {
            SelectedRegions = list;
        }

        public GameBeginningRound()
        {
        }

        /// <summary>
        ///     Verifies correctness of game beginning rounds.
        /// </summary>
        /// <param name="rounds">Rounds from different players.</param>
        /// <returns>Linearized round.</returns>
        public static GameBeginningRound Process(IList<GameBeginningRound> rounds)
        {
            // verify if two players chose one region, if so, return null
            {
                bool doesCollide = (from round in rounds
                                    select round.SelectedRegions
                                    into regions
                                    from region in regions
                                    group region by region.Item2).Any(g => g.Count() > 1);

                if (doesCollide)
                {
                    return null;
                }
            }

            IEnumerable<Tuple<Player, Region>> linearizedRegions = from round in rounds
                                                                   select round.SelectedRegions
                                                                   into regions
                                                                   from region in regions
                                                                   select region;

            GameBeginningRound newRound = new GameBeginningRound(linearizedRegions.ToList());

            return newRound;
        }
    }
}
