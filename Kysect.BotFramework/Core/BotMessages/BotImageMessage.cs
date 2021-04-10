using Kysect.BotFramework.ApiProviders;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotImageMessage : IBotMessage
    {
        public string Text { get; }
        public string ImagePath { get; }

        public BotImageMessage(string text, string imagePath)
        {
            Text = text;
            ImagePath = imagePath;
        }
        
        public void Send(IBotApiProvider apiProvider, SenderInfo sender)
        {
            apiProvider.SendImage(ImagePath, Text, sender);
        }
    }
}