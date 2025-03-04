﻿using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotSingleMediaMessage : IBotMessage
    {
        public IBotMediaFile MediaFile { get; }

        public BotSingleMediaMessage(string text, IBotMediaFile mediaFile)
        {
            Text = text;
            MediaFile = mediaFile;
        }

        public string Text { get; }

        public void Send(IBotApiProvider apiProvider, SenderInfo sender)
        {
            if (MediaFile is IBotOnlineFile onlineFile)
            {
                apiProvider.SendOnlineMedia(onlineFile, Text, sender);
            }
            else
            {
                apiProvider.SendMedia(MediaFile, Text, sender);
            }
        }
    }
}