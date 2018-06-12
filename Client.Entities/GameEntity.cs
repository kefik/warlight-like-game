namespace Client.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.IO;

    public abstract class GameEntity : Entity, IInserted, IDeleted
    {
        public class GameEntityMapper : EntityTypeConfiguration<GameEntity>
        {
            public GameEntityMapper()
            {
                Property(x => x.SavedGameDateString).HasColumnName(nameof(SavedGameDate));
                Property(x => x.FileName).HasColumnName("Path");
            }
        }
        protected abstract string SavedGamesStoragePath { get; }

        protected byte[] Data { get; set; }

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
        public string FileName { get; set; }
        
        [NotMapped]
        internal virtual string Path
        {
            get { return $"{SavedGamesStoragePath}/{FileName}"; }
        }

        /// <summary>
        /// Obtains bytes of file representing this saved game.
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetFileBytes()
        {
            if (Data != null)
            {
                return Data;
            }

            return File.Exists(Path) ? File.ReadAllBytes(Path) : null;
        }

        /// <summary>
        /// Writes data into <see cref="Path"/> location from <seealso cref="Data"/>, if there
        /// is any.
        /// </summary>
        void IInserted.Inserted()
        {
            if (Data != null)
            {
                File.WriteAllBytes(Path, Data);
            }
        }

        /// <summary>
        /// Deletes file described by <see cref="Path"/>.
        /// </summary>
        void IDeleted.Deleted()
        {
            File.Delete(Path);
        }
    }
}