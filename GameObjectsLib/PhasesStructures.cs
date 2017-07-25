using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectsLib.GameMap;
using ProtoBuf;

namespace GameObjectsLib
{
    /// <summary>
    /// Represents deploying phase of the game.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public struct Deploying
    {
        /// <summary>
        /// Represents armies deployed in the deploying phase in given regions.
        /// Int represents armies that will be occuppying this region after this stage.
        /// </summary>
        public List<Tuple<Region, int>> ArmiesDeployed { get; }

        public Deploying(List<Tuple<Region, int>> armiesDeployed)
        {
            ArmiesDeployed = armiesDeployed;
        }  
    }
    /// <summary>
    /// Represents attacking phase of the game.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public struct Attacking
    {
        /// <summary>
        /// Represents attacks that happen during attacking phase.
        /// </summary>
        public List<Attack> Attacks { get; }

        public Attacking(List<Attack> attacks)
        {
            Attacks = attacks;
        }
    }
    
    /// <summary>
    /// Represents one attack in the game round.
    /// </summary>
    [ProtoContract]
    public struct Attack
    {
        /// <summary>
        /// Represents attacking region.
        /// </summary>
        [ProtoMember(1, AsReference = true)]
        public Region Attacker { get; }
        /// <summary>
        /// Attacking army, must be lower or equal than Attacker region army.
        /// </summary>
        [ProtoMember(2)]
        public int AttackingArmy { get; }
        /// <summary>
        /// Defending region.
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
