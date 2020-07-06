using System;
using BotFramework.BotFramework;
using BotFramework.Common;

namespace BotFramework.Abstractions
{
    public interface IBotApiProvider : IWriteMessage
    {
        public event EventHandler<BotEventArgs> OnMessage;
        public void Dispose();
        public Result Initialize();
    }
}