namespace Server.WarlightLikeDatabase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class OpenedGame
    {
        [Key]
        public int Id { get; set; }

        public int OpenedSlotsNumber { get; set; }
    }
}
