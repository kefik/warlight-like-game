namespace Client.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Configuration;

    [Table(nameof(MapInfo) + "s")]
    public class MapInfo : NamedEntity
    {
        private static readonly string mapsStoragePath = ConfigurationManager.AppSettings["MapsStoragePath"];

        public int PlayersLimit { get; set; }

        public string TemplatePath
        {
            get { return $"{mapsStoragePath}/{TemplateName}"; }
        }
        public string TemplateName { get; set; }

        public string ImagePath
        {
            get { return $"{mapsStoragePath}/{ImageName}"; }
        }
        public string ImageName { get; set; }
        
        public string ImageColoredRegionsPath
        {
            get { return $"{mapsStoragePath}/{ColoredRegionsImageName}"; }
        }
        public string ColoredRegionsImageName { get; set; }

        public string ColorRegionsTemplatePath
        {
            get { return $"{mapsStoragePath}/{ColoredRegionsTemplateName}"; }
        }
        public string ColoredRegionsTemplateName { get; set; }
    }
}
