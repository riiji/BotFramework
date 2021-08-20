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
            var contextSenderInfo = DiscordSenderInfoEntity.GetOrCreate(this, dbContext);

            var contextModel = DialogContextEntity.GetOrCreate(contextSenderInfo, ContextType.Discord, dbContext);
            
            return new DialogContext(contextModel.State, contextModel.SenderInfoId, ContextType.Discord, this);
        }
    }
}