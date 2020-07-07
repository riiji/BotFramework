using System;
using Tef.BotFramework.Common;
using Tef.BotFramework.Core;

namespace Tef.BotFramework.Abstractions
{
    public interface IBotApiProvider : IWriteMessage
    {
        public event EventHandler<BotEventArgs> OnMessage;
        public void OnFail();
    }
}