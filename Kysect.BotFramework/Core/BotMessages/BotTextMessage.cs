using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotTextMessage : IBotMessage
    {
        public BotTextMessage(string text)
        {
            Text = text;
        }

        public string Text { get; }

        public void Send(IBotApiProvider apiProvider, SenderInfo sender)
        {
            apiProvider.SendTextMessage(Text, sender);
        }
    }
}