using Kysect.BotFramework.ApiProviders;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotImageAndVideoMessage : IBotMessage
    {
        public string Text { get; }
        public string ImagePath { get; }

        public string VideoPath { get; }
        
        public BotImageAndVideoMessage(string text, string imagePath, string videoPath)
        {
            Text = text;
            ImagePath = imagePath;
            VideoPath = videoPath;
        }
        
        public void Send(IBotApiProvider apiProvider, SenderInfo sender)
        {
            apiProvider.SendImage(ImagePath, Text, sender);
            apiProvider.SendVideo(VideoPath, "", sender);
        }
    }
}