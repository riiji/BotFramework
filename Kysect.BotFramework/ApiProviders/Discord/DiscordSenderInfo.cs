using System.Linq;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Data;
using Kysect.BotFramework.Data.Entities;

namespace Kysect.BotFramework.ApiProviders.Discord
{
    public class DiscordSenderInfo : SenderInfo
    {
        public ulong GuildId { get; }

        public DiscordSenderInfo(long chatId, long userSenderId, string userSenderUsername, bool isAdmin, ulong guildId)
            : base(chatId, userSenderId, userSenderUsername, isAdmin)
        {
            GuildId = guildId;
        }

        private DiscordSenderInfoEntity ToEntity()
        {
            var entity = new DiscordSenderInfoEntity
            {
                GuildId = GuildId,
                ChatId = ChatId,
                UserSenderId = UserSenderId
            };
            return entity;
        }

        internal override DialogContext GetOrCreateDialogContext(BotFrameworkDbContext dbContext)
        {
            DiscordSenderInfoEntity contextSenderInfo =  dbContext.DiscordSenderInfos.FirstOrDefault(si =>
                si.GuildId == GuildId && si.ChatId == ChatId && si.UserSenderId == UserSenderId
            );
            if (contextSenderInfo is null)
            {
                DiscordSenderInfoEntity entity = ToEntity();
                dbContext.DiscordSenderInfos.AddAsync(entity);
                dbContext.SaveChangesAsync();

                var context = new DialogContextEntity
                {
                    SenderInfoId = entity.Id,
                    ContextType = ContextType.Discord
                };

                dbContext.Add(context);
                dbContext.SaveChanges();

                return new DialogContext(context.State, context.SenderInfoId, ContextType.Discord, this);
            }

            DialogContextEntity contextModel = dbContext.DialogContexts.FirstOrDefault(c => 
                c.SenderInfoId == contextSenderInfo.Id && c.ContextType == ContextType.Discord);
            if (contextModel is null)
            {
                contextModel = new DialogContextEntity
                {
                    SenderInfoId = contextSenderInfo.Id,
                    ContextType = ContextType.Discord
                };

                dbContext.Add(contextModel);
                dbContext.SaveChanges();
            }
            return new DialogContext(contextModel.State, contextModel.SenderInfoId, ContextType.Discord, this);
        }
    }
}