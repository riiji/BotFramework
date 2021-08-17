using System.Linq;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Data;
using Kysect.BotFramework.Data.Entities;

namespace Kysect.BotFramework.ApiProviders.Telegram
{
    public class TelegramSenderInfo : SenderInfo
    {
        public TelegramSenderInfo(long chatId, long userSenderId, string userSenderUsername, bool isAdmin)
            : base(chatId, userSenderId, userSenderUsername, isAdmin)
        {
        }
        
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

                var context = new DialogContextEntity();
                context.SenderInfoId = entity.Id;

                dbContext.Add(context);
                dbContext.SaveChanges();

                return new DialogContext(context.State, context.SenderInfoId, this);
            }

            var contextModel = dbContext.DialogContexts.FirstOrDefault(c=>c.SenderInfoId == contextSenderInfo.Id);
            if (contextModel is null)
            {
                contextModel = new DialogContextEntity
                {
                    SenderInfoId = contextSenderInfo.Id
                };

                dbContext.Add(contextModel);
                dbContext.SaveChanges();
            }
            return new DialogContext(contextModel.State, contextModel.SenderInfoId, this);
        }
    }
}