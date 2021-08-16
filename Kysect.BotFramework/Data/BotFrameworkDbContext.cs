using Kysect.BotFramework.ApiProviders.Discord;
using Kysect.BotFramework.ApiProviders.Telegram;
using Kysect.BotFramework.Core.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Kysect.BotFramework.Data
{
    public class BotFrameworkDbContext : DbContext
    {
        public DbSet<DialogContextModel> DialogContexts { get; set; }
        public DbSet<DiscordSenderInfo> DiscordSenderInfos { get; set; }
        public DbSet<TelegramSenderInfo> TelegramSenderInfos { get; set; }
        
        public BotFrameworkDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DialogContextModel>().HasKey(d => d.SenderInfoId);
            modelBuilder.Entity<DiscordSenderInfo>().HasKey(d => d.Id);
            modelBuilder.Entity<TelegramSenderInfo>().HasKey(t => t.Id);
        }
    }
}