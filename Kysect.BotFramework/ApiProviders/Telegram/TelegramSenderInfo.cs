using System.Linq;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Data;

namespace Kysect.BotFramework.ApiProviders.Telegram
{
    public class TelegramSenderInfo : SenderInfo
    {
        public TelegramSenderInfo() : base()
        {
            
        }
        
        public TelegramSenderInfo(long chatId, long userSenderId, string userSenderUsername, bool isAdmin)
            : base(chatId, userSenderId, userSenderUsername, isAdmin)
        {
        }

        internal override DialogContext GetDialogContext(BotFrameworkDbContext dbContext)
        {
            var contextSenderInfo =  dbContext.TelegramSenderInfos.FirstOrDefault(si => 
                si.ChatId == ChatId && si.UserSenderId == UserSenderId);
            if (contextSenderInfo is null)
            {
                dbContext.TelegramSenderInfos.AddAsync(this);
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
                dbContext.TelegramSenderInfos.Update(this);
                dbContext.SaveChanges();    
            }
            
            var contextModel = dbContext.DialogContexts.Find(this.Id);
            return new DialogContext(contextModel.State, this);
        }
    }
}