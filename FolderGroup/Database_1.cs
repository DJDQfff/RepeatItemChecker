using Microsoft.EntityFrameworkCore;

namespace FolderGroupDB
{
    public class Database_1 : DbContext
    {
        public DbSet<FoldersGroup> folderGroups { get; set; }

        private string connectString = "Data Source=DB_1.db";

        public Database_1 ()
        { }

        public Database_1 (string connectString)
        {
            this.connectString = connectString;
        }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(connectString);
        }
    }
}