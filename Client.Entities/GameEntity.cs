namespace Client.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.IO;

    public abstract class GameEntity : NamedEntity
    {
        protected abstract string SavedGamesStoragePath { get; }

        public virtual int AiNumber { get; set; }

        [StringLength(40)]
        public virtual string MapName { get; set; }
        
        public virtual string SavedGameDate { get; set; }

        [StringLength(40)]
        [Column("Path")]
        public override string Name { get; set; }
        
        [NotMapped]
        public string Path
        {
            get { return $"{SavedGamesStoragePath}/{Name}"; }
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