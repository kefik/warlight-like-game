namespace Server.WarlightLikeDatabase
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class MapInfo : NamedEntity
    {
        [Required]
        [StringLength(50)]
        [Index(IsUnique = true)]
        public override string Name { get; set; }

        [Required]
        public int PlayersLimit { get; set; }
        [Required]
        [StringLength(100)]
        public string TemplatePath { get; set; }
        [Required]
        [StringLength(100)]
        public string ImagePath { get; set; }
        [Required]
        [StringLength(100)]
        public string ImageColoredRegionsPath { get; set; }
        [Required]
        [StringLength(100)]
        public string ColorRegionsTemplatePath { get; set; }
    }
}
