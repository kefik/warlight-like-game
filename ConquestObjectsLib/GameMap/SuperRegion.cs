using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ConquestObjectsLib.GameMap
{
    /// <summary>
    /// Represents region group of giving size giving bonus to player who owns it all.
    /// </summary>
    public class SuperRegion
    {
        public int Id { get; }
        /// <summary>
        /// Name given of the SuperRegion.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Bonus given by the SuperRegion taken by one player.
        /// </summary>
        public int Bonus { get; }

        public Player Owner { get; private set; } // TODO: finish refreshing of situation, use GetOwner() method

        public ICollection<Region> Regions { get; }= new HashSet<Region>();
        /// <summary>
        /// Constructs SuperRegion instance.
        /// </summary>
        /// <param name="name">Name of the new SuperRegion.</param>
        /// <param name="bonus">Bonus given by SuperRegion.</param>
        /// <param name="id">Id identifying the SuperRegion.</param>
        public SuperRegion(int id, string name, int bonus)
        {
            Id = id;
            Name = name;
            Bonus = bonus;
        }
        /// <summary>
        /// Calculates owner of this SuperRegion.
        /// </summary>
        /// <returns>Owner of this SuperRegion, or null.</returns>
        private Player GetOwner()
        {
            var firstOwner = Regions.FirstOrDefault()?.Owner;
            return (from region in Regions where region.Owner != firstOwner select region).Any() // if there exists any such that hes not same as the first owner
                ? null // return null
                : firstOwner;
        }

        public override string ToString()
        {
            string name = string.Format($"{nameof(Name)}: {Name}");
            string bonus = string.Format($"{nameof(Bonus)}: {Bonus}");
            string regions;
            {
                var sb = new StringBuilder();
                foreach (var region in Regions)
                {
                    sb.Append(region.Name + ", ");
                }
                regions = string.Format($"{nameof(Regions)}: {sb}");
            }
            return name + ", " + bonus + ", " + regions;
        }
        
    }
}
