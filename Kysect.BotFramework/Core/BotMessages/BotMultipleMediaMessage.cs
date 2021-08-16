using System.Collections.Generic;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotMultipleMediaMessage : IBotMessage
    {
        public List<IBotMediaFile> MediaFiles { get; }

        public BotMultipleMediaMessage(string text, List<IBotMediaFile> mediaFiles)
        {
            Text = text;
            MediaFiles = mediaFiles;
        }

        public string Text { get; }

        public void Send(IBotApiProvider apiProvider, SenderInfo sender)
        {
            apiProvider.SendMultipleMedia(MediaFiles, Text, sender);
        }
    }
}