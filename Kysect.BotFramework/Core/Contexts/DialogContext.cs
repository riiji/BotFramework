using System;
using System.Linq;
using Kysect.BotFramework.Data;
using Kysect.BotFramework.Data.Entities;

namespace Kysect.BotFramework.Core.Contexts
{
    public class DialogContext
    {
        public int State { get; set; }
        private long _senderInfoId;
        public SenderInfo SenderInfo { get; }

        public DialogContext(int state, long senderInfoId, SenderInfo senderInfo)
        {
            State = state;
            _senderInfoId = senderInfoId;
            SenderInfo = senderInfo;
        }

        internal void Update(BotFrameworkDbContext dbContext)
        {
            DialogContextEntity context = dbContext.DialogContexts.FirstOrDefault(x => x.SenderInfoId == _senderInfoId);
            context.State = State;
            dbContext.DialogContexts.Update(context);
            dbContext.SaveChanges();
        }
    }
}