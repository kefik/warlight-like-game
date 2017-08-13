namespace Server.WarlightLikeDatabase
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class MapInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Index(IsClustered = false, IsUnique = false)]
        public string Name { get; set; }
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
