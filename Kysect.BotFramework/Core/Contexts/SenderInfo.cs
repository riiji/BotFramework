using Kysect.BotFramework.Data;

namespace Kysect.BotFramework.Core.Contexts
{
    public abstract class SenderInfo
    {
        //TODO: Split to model and buisness logic instance
        internal int Id { get;  set; }
        public long ChatId { get; internal set; }
        public long UserSenderId { get; internal set; }
        public string UserSenderUsername { get; internal set; }
        public bool IsAdmin { get; internal set; }

        public SenderInfo()
        {
            
        }
        
        protected SenderInfo(long chatId, long userSenderId, string userSenderUsername, bool isAdmin)
        {
            ChatId = chatId;
            UserSenderId = userSenderId;
            UserSenderUsername = userSenderUsername;
            IsAdmin = isAdmin;
        }

        internal abstract DialogContext GetDialogContext(BotFrameworkDbContext dbContext);
    }
}