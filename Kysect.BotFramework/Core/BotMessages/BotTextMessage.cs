using Kysect.BotFramework.ApiProviders;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotTextMessage : IBotMessage
    {
        public string Text { get; }
        public void Send(IBotApiProvider apiProvider, BotEventArgs sender)
        {
            throw new System.NotImplementedException();
        }

        public  BotTextMessage(string text)
        {
            Text = text;
        }
    }
}