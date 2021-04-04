using System;
using FluentResults;
using Kysect.BotFramework.Core;

namespace Kysect.BotFramework.ApiProviders
{
    public interface IBotApiProvider
    {
        public event EventHandler<BotEventArgs> OnMessage;

        public void Restart();
        public Result<string> WriteMessage(BotEventArgs sender);
    }
}