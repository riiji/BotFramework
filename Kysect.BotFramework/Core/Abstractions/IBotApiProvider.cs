using System;
using FluentResults;

namespace Kysect.BotFramework.Core.Abstractions
{
    public interface IBotApiProvider
    {
        public event EventHandler<BotEventArgs> OnMessage;

        public void Restart();
        public Result<string> WriteMessage(BotEventArgs sender);
    }
}