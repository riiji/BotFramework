using System;
using System.Collections.Generic;
using FluentResults;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.ApiProviders
{
    public interface IBotApiProvider
    {
        event EventHandler<BotNewMessageEventArgs> OnMessage;

        void Restart();
        Result<string> SendMultipleMedia(List<IBotMediaFile> mediaFiles, string text, SenderInfo sender);
        Result<string> SendMedia(IBotMediaFile mediaFile, string text, SenderInfo sender);
        Result<string> SendOnlineMedia(IBotOnlineFile file, string text, SenderInfo sender);
        Result<string> SendTextMessage(string text, SenderInfo sender);
    }
}