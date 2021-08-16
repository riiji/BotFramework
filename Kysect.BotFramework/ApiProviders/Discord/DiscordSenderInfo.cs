using System.Linq;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Data;

namespace Kysect.BotFramework.ApiProviders.Discord
{
    public class DiscordSenderInfo : SenderInfo
    {
        public ulong GuildId { get; internal set; }

        public DiscordSenderInfo() : base()
        {
            
        }
        
        public DiscordSenderInfo(long chatId, long userSenderId, string userSenderUsername, bool isAdmin, ulong guildId)
            : base(chatId, userSenderId, userSenderUsername, isAdmin)
        {
            GuildId = guildId;
        }


        internal override DialogContext GetDialogContext(BotFrameworkDbContext dbContext)
        {
            var contextSenderInfo =  dbContext.DiscordSenderInfos.FirstOrDefault(si =>
                si.GuildId == GuildId && si.ChatId == ChatId && si.UserSenderId == UserSenderId
            );
            if (contextSenderInfo is null)
            {
                dbContext.DiscordSenderInfos.AddAsync(this);
                dbContext.SaveChangesAsync();

                var context = new DialogContextModel();
                context.SenderInfoId = Id;

                dbContext.Add(context);
                dbContext.SaveChanges();

                return new DialogContext(context.State, this);
            }
            
            this.Id = contextSenderInfo.Id;
            if (contextSenderInfo.IsAdmin != IsAdmin || contextSenderInfo.UserSenderUsername != UserSenderUsername)
            {
                dbContext.DiscordSenderInfos.Update(this);
                dbContext.SaveChanges();    
            }

            var contextModel = dbContext.DialogContexts.FirstOrDefault(c => c.SenderInfoId == this.Id);
            return new DialogContext(contextModel.State, this);
        }
    }
}