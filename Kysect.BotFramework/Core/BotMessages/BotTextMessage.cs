using Kysect.BotFramework.ApiProviders;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotTextMessage : IBotMessage
    {
        public string Text { get; }
        public void Send(IBotApiProvider apiProvider, SenderInfo sender)
        {
            apiProvider.SendTextMessage(Text, sender);
        }

        public  BotTextMessage(string text)
        {
            Text = text;
        }
    }
}