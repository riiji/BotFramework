using System;
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
        private readonly DatabaseType _type;
        
        public DatabaseContext(string connectionString, DatabaseType type)
        {
            _connectionString = connectionString;
            _type = type;
        }

        private void ConfigureSqlite(DbContextOptionsBuilder options)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = _connectionString };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            options.UseSqlite(connection);
        }

        private void ConfigurePostgreSql(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(_connectionString);
        }

        private void ConfigureSql(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_connectionString);
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (_type)
            {
                case DatabaseType.Sqlite:
                    ConfigureSqlite(optionsBuilder);
                    break;
                case DatabaseType.PostgreSql:
                    ConfigurePostgreSql(optionsBuilder);
                    break;
                case DatabaseType.Sql:
                    ConfigureSql(optionsBuilder);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum DatabaseType
    {
        Sqlite,
        PostgreSql,
        Sql,
    }
}