using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tef.BotFramework.Database.Models;

namespace Tef.BotFramework.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<BotSettings> BotSettings { get; set; }
        public DbSet<EventSettings> EventSettings { get; set; }
        public DbSet<GroupSettings> GroupSettings { get; set; }

        private readonly string _connectionString;
        
        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
        }
       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = _connectionString };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }
    }
}