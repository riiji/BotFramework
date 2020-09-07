using System;
using FluentResults;
using Tef.BotFramework.Core;

namespace Tef.BotFramework.Abstractions
{
    public interface IBotApiProvider
    {
        public event EventHandler<BotEventArgs> OnMessage;
        public void Restart();
        public Result<string> WriteMessage(BotEventArgs sender);
    }
}