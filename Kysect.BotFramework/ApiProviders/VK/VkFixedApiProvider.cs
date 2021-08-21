using System;
using System.Collections.Generic;
using FluentResults;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.ApiProviders.VK
{
    public class VkFixedApiProvider : IBotApiProvider, IDisposable
    {
        public event EventHandler<BotNewMessageEventArgs> OnMessage;
        public void Restart()
        {
            throw new NotImplementedException();
        }

        public Result<string> SendMultipleMedia(List<IBotMediaFile> mediaFiles, string text, SenderInfo sender) => throw new NotImplementedException();

        public Result<string> SendMedia(IBotMediaFile mediaFile, string text, SenderInfo sender) => throw new NotImplementedException();

        public Result<string> SendOnlineMedia(IBotOnlineFile file, string text, SenderInfo sender) => throw new NotImplementedException();

        public Result<string> SendTextMessage(string text, SenderInfo sender) => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}