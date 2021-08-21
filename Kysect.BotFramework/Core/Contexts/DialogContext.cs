using System.Linq;
using Kysect.BotFramework.Data;
using Kysect.BotFramework.Data.Entities;

namespace Kysect.BotFramework.Core.Contexts
{
    public class DialogContext
    {
        public int State { get; set; }
        private readonly ContextType _contextType;
        private readonly long _senderInfoId;
        public SenderInfo SenderInfo { get; }

        public DialogContext(int state, long senderInfoId, ContextType contextType, SenderInfo senderInfo)
        {
            State = state;
            _senderInfoId = senderInfoId;
            _contextType = contextType;
            SenderInfo = senderInfo;
        }

        internal void SaveChanges(BotFrameworkDbContext dbContext)
        {
            DialogContextEntity context = dbContext.DialogContexts.FirstOrDefault(x => x.SenderInfoId == _senderInfoId && x.ContextType == _contextType);
            context.State = State;
            dbContext.DialogContexts.Update(context);
            dbContext.SaveChanges();
        }
    }
}