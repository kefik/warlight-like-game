using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectsLib.GameMap;

namespace GameObjectsLib
{
    public class Round
    {
        public int Number { get; }
        public Deploying Deploying { get; }
        public Attacking Attacking { get; }

        public Round(int number, Deploying deploying, Attacking attacking)
        {
            Number = number;
            Deploying = deploying;
            Attacking = attacking;
        }
        /// <summary>
        /// Processes the rounds from different players, ordering them correctly.
        /// Rounds in parameters must always be one from the same player.
        /// </summary>
        /// <param name="rounds">Rounds, each from one certain player.</param>
        /// <returns>Round consisting of rounds in parameter, ordered differently.</returns>
        public static Round Process(params Round[] rounds)
        {
            // TODO

            var deploying = new Deploying(new List<Tuple<Region, int>>());
            var attacking = new Attacking(new List<Attack>());

            int index = 0;
            while (true)
            {
                foreach (var round in rounds)
                {
                    Tuple<Region, int> armyDeployed;
                    if (round.Deploying.ArmiesDeployed.Count < index)
                    {
                        armyDeployed = round.Deploying.ArmiesDeployed[index];
                    }

                    Attack attack;
                    if (round.Attacking.Attacks.Count < index)
                    {
                        attack = round.Attacking.Attacks[index];
                    }
                }
                index++;
            }
            
            throw new NotImplementedException();
        }
    }
}
