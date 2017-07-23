using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectsLib.GameMap;

namespace GameObjectsLib
{
    /// <summary>
    /// Instance of this place represents a game round, which consists of each
    /// players round. Round identifies what has happened between two game states.
    /// </summary>
    public class Round
    {
        /// <summary>
        /// Represents number of the round in the game.
        /// Round 0 represents special game round, where
        /// players only choose their regions.
        /// </summary>
        public int Number { get; }
        /// <summary>
        /// Represents deploying phase of the game round.
        /// </summary>
        public Deploying Deploying { get; set; }
        /// <summary>
        /// Represents attacking phase of the game round.
        /// </summary>
        public Attacking Attacking { get; set; }

        public Round(int number, Deploying deploying, Attacking attacking)
        {
            Number = number;
            Deploying = deploying;
            Attacking = attacking;
        }

        public Round(int number)
        {
            Number = number;
        }
        /// <summary>
        /// Processes the round from different players, ordering them correctly.
        /// Rounds in parameters must always be one from the same player.
        /// </summary>
        /// <param name="rounds">Rounds, each from one certain player.</param>
        /// <returns>Round consisting of round in parameter, ordered differently.</returns>
        public static Round Process(params Round[] rounds)
        {
            // validate that rounds have same number

            //{
            //    var first = rounds.First();

            //    bool anyWithDifferentRoundNumber = (from round in rounds
            //                 where round.Number != first.Number
            //                 select round).Any();

            //    if (anyWithDifferentRoundNumber) throw new ArgumentException();
            //}

            // TODO: check

            var deploying = new Deploying(new List<Tuple<Region, int>>());
            var attacking = new Attacking(new List<Attack>());

            int index = 0;
            while (true)
            {
                // true if anything in this round of cycle was added
                // false if nothing happened => everything was solved => can break out
                bool didSomething = false;
                foreach (var round in rounds)
                {
                    if (round.Deploying.ArmiesDeployed.Count < index)
                    {
                        var armyDeployed = round.Deploying.ArmiesDeployed[index];
                        deploying.ArmiesDeployed.Add(armyDeployed);
                        didSomething = true;
                    }

                    if (round.Attacking.Attacks.Count < index)
                    {
                        var attack = round.Attacking.Attacks[index];
                        attacking.Attacks.Add(attack);
                        didSomething = true;
                    }
                }

                if (!didSomething) break;

                index++;
            }
            
            return new Round(rounds[0].Number, deploying, attacking);
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

    public class GameBeginningRound
    {
        public List<Tuple<Player, Region>> SelectedRegions { get; } = new List<Tuple<Player, Region>>();
    }

}
