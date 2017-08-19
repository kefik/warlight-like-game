using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.WarlightLikeDatabase
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class GameEntity : Entity
    {
        [Required]
        [StringLength(50)]
        public string MapName { get; set; }

        [Required]
        public int AiPlayersCount { get; set; }

        [Required]
        public int HumanPlayersCount { get; set; }

        [Required]
        public virtual ICollection<User> Users { get; set; }
    }
}
