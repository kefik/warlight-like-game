namespace Server.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

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
