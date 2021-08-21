using System.Linq;
using Kysect.BotFramework.ApiProviders.Discord;

namespace Kysect.BotFramework.Data.Entities
{
    public class DiscordSenderInfoEntity : SenderInfoEntity
    {
        public ulong GuildId { get; set; }

        public static DiscordSenderInfoEntity GetOrCreate(DiscordSenderInfo senderInfo, BotFrameworkDbContext dbContext)
        {
            var senderInfoEntity = dbContext.DiscordSenderInfos.FirstOrDefault(
                si =>
                    si.GuildId == senderInfo.GuildId
                    && si.ChatId == senderInfo.ChatId
                    && si.UserSenderId == senderInfo.UserSenderId
            );

            if (senderInfoEntity is null)
            {
                senderInfoEntity = new DiscordSenderInfoEntity()
                {
                    ChatId = senderInfo.ChatId,
                    GuildId = senderInfo.GuildId,
                    UserSenderId = senderInfo.UserSenderId
                };

                dbContext.DiscordSenderInfos.Add(senderInfoEntity);
                dbContext.SaveChanges();
            }

            return senderInfoEntity;
        }
    }
}