using System;
using System.Linq;
using Kysect.BotFramework.Data;

namespace Kysect.BotFramework.Core.Contexts
{
    public class DialogContext
    {
        public int State { get; set; }
        public SenderInfo SenderInfo { get; }

        public DialogContext(int state, SenderInfo senderInfo)
        {
            State = state;
            SenderInfo = senderInfo;
        }

        internal void Update(BotFrameworkDbContext dbContext)
        {
            var context = dbContext.DialogContexts.FirstOrDefault(x => x.SenderInfoId == SenderInfo.Id);
            context.State = State;
            dbContext.DialogContexts.Update(context);
            dbContext.SaveChanges();
        }
    }
}