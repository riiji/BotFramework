using System.Linq;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Data;
using Kysect.BotFramework.Data.Entities;

namespace Kysect.BotFramework.ApiProviders.Telegram
{
    public class TelegramSenderInfo : SenderInfo
    {
        public TelegramSenderInfo(long chatId, long userSenderId, string userSenderUsername, bool isAdmin)
            : base(chatId, userSenderId, userSenderUsername, isAdmin)
        { }
        
        private TelegramSenderInfoEntity ToEntity()
        {
            var entity = new TelegramSenderInfoEntity()
            {
                ChatId = ChatId,
                UserSenderId = UserSenderId
            };
            return entity;
        }

        internal override DialogContext GetDialogContext(BotFrameworkDbContext dbContext)
        {
            TelegramSenderInfoEntity contextSenderInfo =  dbContext.TelegramSenderInfos.FirstOrDefault(si => 
                si.ChatId == ChatId && si.UserSenderId == UserSenderId);
            if (contextSenderInfo is null)
            {
                TelegramSenderInfoEntity entity = ToEntity();
                dbContext.TelegramSenderInfos.AddAsync(entity);
                dbContext.SaveChangesAsync();

                var context = new DialogContextEntity
                {
                    SenderInfoId = entity.Id,
                    ContextType = ContextType.Telegram
                };

                dbContext.Add(context);
                dbContext.SaveChanges();

                return new DialogContext(context.State, context.SenderInfoId, ContextType.Telegram, this);
            }

            DialogContextEntity contextModel = dbContext.DialogContexts.FirstOrDefault(c=> 
                c.SenderInfoId == contextSenderInfo.Id && c.ContextType == ContextType.Telegram);
            if (contextModel is null)
            {
                contextModel = new DialogContextEntity
                {
                    SenderInfoId = contextSenderInfo.Id,
                    ContextType = ContextType.Telegram
                };

                dbContext.Add(contextModel);
                dbContext.SaveChanges();
            }
            return new DialogContext(contextModel.State, contextModel.SenderInfoId, ContextType.Telegram, this);
        }
    }
}