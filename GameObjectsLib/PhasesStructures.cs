using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectsLib.GameMap;

namespace GameObjectsLib
{
    public struct Deploying
    {
        public List<Tuple<Region, int>> ArmiesDeployed { get; }

        public Deploying(List<Tuple<Region, int>> armiesDeployed)
        {
            ArmiesDeployed = armiesDeployed;
        }  
    }

    public struct Attacking
    {
        public List<Attack> Attacks { get; }

        public Attacking(List<Attack> attacks)
        {
            Attacks = attacks;
        }
    }
    
    public struct Attack
    {
        public Region Attacker { get; }
        public int AttackingArmy { get; }

        public Region Defender { get; }

        public Attack(Region attacker, int attackingArmy, Region defender)
        {
            Attacker = attacker;
            AttackingArmy = attackingArmy;

            Defender = defender;
        }
    }
}
