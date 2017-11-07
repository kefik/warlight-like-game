namespace Client.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.IO;

    public abstract class GameEntity : NamedEntity
    {
        public class GameEntityMapper : EntityTypeConfiguration<GameEntity>
        {
            public GameEntityMapper()
            {
                Property(x => x.SavedGameDateString).HasColumnName(nameof(SavedGameDate));
                Property(x => x.Name).HasColumnName("Path");
            }
        }
        protected abstract string SavedGamesStoragePath { get; }

        public virtual int AiNumber { get; set; }

        [StringLength(40)]
        public virtual string MapName { get; set; }

        [NotMapped]
        public virtual DateTime SavedGameDate
        {
            get { return DateTime.Parse(SavedGameDateString); }
            set
            {
                SavedGameDateString = value.ToString();
            }
        }

        protected string SavedGameDateString { get; set; }

        [StringLength(40)]
        public override string Name { get; set; }
        
        [NotMapped]
        public string Path
        {
            get { return $"{SavedGamesStoragePath}/{Name}"; }
        }

        /// <summary>
        /// Obtains bytes of file representing this saved game.
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetFileBytes()
        {
            return File.Exists(Path) ? File.ReadAllBytes(Path) : null;
        }

        public virtual void Inserted()
        {
            using (FileStream fs = new FileStream(Path, FileMode.Create))
            {
                // TODO: serialize game
                // fs.Write();
            }
        }

        public virtual void Deleted()
        {
            File.Delete(Path);
        }
    }
}