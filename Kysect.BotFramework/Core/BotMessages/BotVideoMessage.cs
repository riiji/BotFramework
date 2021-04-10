using Kysect.BotFramework.ApiProviders;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotVideoMessage : IBotMessage
    {
        public string Text { get; }
        public string VideoPath { get; }

        public BotVideoMessage(string text, string imagePath)
        {
            Text = text;
            VideoPath = imagePath;
        }
        
        public void Send(IBotApiProvider apiProvider, SenderInfo sender)
        {
            apiProvider.SendVideo(VideoPath, Text, sender);
        }
    }
}