using System;
using System.Collections.Generic;
using FluentResults;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;

namespace Kysect.BotFramework.ApiProviders
{
    public interface IBotApiProvider
    {
        event EventHandler<BotEventArgs> OnMessage;

        void Restart();
        public Result<string> SendText(string text, SenderInfo sender);
        public Result<string> SendMedia(IBotMediaFile mediaFile, string text, SenderInfo sender);
        public Result<string> SendMultipleMedia(List<IBotMediaFile> mediaFiles, string text, SenderInfo sender);

        public Result<string> SendOnlineMedia(IBotOnlineFile file, string text, SenderInfo sender);
    }
}