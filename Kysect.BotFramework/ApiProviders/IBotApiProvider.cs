using System;
using FluentResults;
using Kysect.BotFramework.Core;

namespace Kysect.BotFramework.ApiProviders
{
    public interface IBotApiProvider
    {
        event EventHandler<BotEventArgs> OnMessage;

        void Restart();
        Result<string> SendText(String text, SenderInfo sender);
    }
}