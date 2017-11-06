namespace GameObjectsLib.GameRecording
{
    using System.Collections.Generic;
    using ProtoBuf;

    /// <summary>
    ///     Instance of this place represents a game round, which consists of each
    ///     players round. Round identifies what has happened between two game states.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public sealed class GameRound : Round
    {
        public GameRound()
        {
        }
        
        public override ILinearizedRound Linearize()
        {
            Deploying deploying = new Deploying(new List<Deployment>());
            Attacking attacking = new Attacking(new List<Attack>());

            int index = 0;
            while (true)
            {
                // true if anything in this round of cycle was added
                // false if nothing happened => everything was solved => can break out
                bool didSomething = false;
                foreach (GameTurn round in Turns)
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
            return new LinearizedGameRound(deploying, attacking);
        }
    }
}
