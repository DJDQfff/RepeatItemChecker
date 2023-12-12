using Microsoft.EntityFrameworkCore;

namespace FolderGroupDB
{
    public class Database_1 : DbContext
    {
        public DbSet<FoldersGroup> folderGroups { get; set; }

        public Database_1 ()
        { }

        public Database_1 (string connectString)
        {
            Configuration.ConnectionString = connectString;
        }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(Configuration.ConnectionString);
        }
    }
}