using System;
using Tef.BotFramework.BotFramework;
using Tef.BotFramework.Common;

namespace Tef.BotFramework.Abstractions
{
    public interface IBotApiProvider : IWriteMessage
    {
        public event EventHandler<BotEventArgs> OnMessage;
        public void Dispose();
        public Result Initialize();
    }
}