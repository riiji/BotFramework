namespace Kysect.BotFramework.Core
{
    public class BotMessage
    {
        public string Text { get; }

        public  BotMessage(string text)
        {
            Text = text;
        }
    }
}