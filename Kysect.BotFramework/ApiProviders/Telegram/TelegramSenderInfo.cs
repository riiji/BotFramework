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

        internal override DialogContext GetOrCreateDialogContext(BotFrameworkDbContext dbContext)
        {
            var contextSenderInfo = TelegramSenderInfoEntity.GetOrCreate(this, dbContext);
            var contextModel = DialogContextEntity.GetOrCreate(contextSenderInfo, ContextType.Telegram, dbContext);
            
            return new DialogContext(contextModel.State, contextModel.SenderInfoId, ContextType.Telegram, this);
        }
    }
}