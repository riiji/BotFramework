using System;
using FluentResults;

namespace Tef.BotFramework.Core.Abstractions
{
    public interface IBotApiProvider
    {
        public event EventHandler<BotEventArgs> OnMessage;

        public void Restart();
        public Result<string> WriteMessage(BotEventArgs sender);
    }
}