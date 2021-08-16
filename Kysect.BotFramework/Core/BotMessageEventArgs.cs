using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Core
{
    public class BotMessageEventArgs
    {
        public IBotMessage Message { get; }
        public SenderInfo SenderInfo { get; }

        public BotMessageEventArgs(IBotMessage message, SenderInfo senderInfo)
        {
            Message = message;
            SenderInfo = senderInfo;
        }
    }
}