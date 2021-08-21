using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Core
{
    public class BotNewMessageEventArgs
    {
        public IBotMessage Message { get; }
        public SenderInfo SenderInfo { get; }

        public BotNewMessageEventArgs(IBotMessage message, SenderInfo senderInfo)
        {
            Message = message;
            SenderInfo = senderInfo;
        }
    }
}