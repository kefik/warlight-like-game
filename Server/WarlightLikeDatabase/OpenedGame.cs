namespace Server.WarlightLikeDatabase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Data.Entity.Spatial;
    using System.IO;
    using System.Threading.Tasks;
    using GameObjectsLib.Game;
    using GameObjectsLib.NetworkCommObjects;

    public class OpenedGame : GameEntity
    {
        [Required]
        public int OpenedSlotsNumber { get; set; }
        
        [Required]
        [StringLength(50)]
        public string GameCreatedDateTime { get; set; }

        [Required]
        public virtual ICollection<User> SignedUsers { get; set; }

    }
}
