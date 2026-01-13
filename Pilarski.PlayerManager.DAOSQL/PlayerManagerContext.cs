using Microsoft.EntityFrameworkCore;
using Pilarski.PlayerManager.Core;
using System.IO;
using System.Linq;

namespace Pilarski.PlayerManager.DAOSQL
{
    public class PlayerManagerContext : DbContext
    {
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            while (!Directory.GetFiles(path, "*.sln").Any())
            {
                var parent = Directory.GetParent(path);
                if (parent == null)
                {
                    path = AppDomain.CurrentDomain.BaseDirectory;
                    break;
                }
                path = parent.FullName;
            }
            string dataFolder = Path.Combine(path, "PlayerManagerData");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            string dbPath = Path.Combine(dataFolder, "player_manager.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
