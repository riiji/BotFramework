using System;
using Tef.BotFramework.Common;
using Tef.BotFramework.Core;

namespace Tef.BotFramework.Abstractions
{
    public interface IBotApiProvider
    {
        public event EventHandler<BotEventArgs> OnMessage;
        public void Restart();
        public Result WriteMessage(BotEventArgs sender);
    }
}