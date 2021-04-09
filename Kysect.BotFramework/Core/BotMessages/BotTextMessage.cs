namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotTextMessage : IBotMessage
    {
        public string Text { get; }

        public  BotTextMessage(string text)
        {
            Text = text;
        }
    }
}