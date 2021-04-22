using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.BotMedia;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotSingleMediaMessage : IBotMessage
    {
        public string Text { get; }
        public IBotMediaFile MediaFile { get; }

        public BotSingleMediaMessage(string text, IBotMediaFile mediaFile)
        {
            Text = text;
            MediaFile = mediaFile;
        }
        
        public void Send(IBotApiProvider apiProvider, SenderInfo sender)
        {
            apiProvider.SendMedia(MediaFile, Text, sender);
        }
    }
}