using Tef.BotFramework.Common;
using Tef.BotFramework.Core;

namespace Tef.BotFramework.Abstractions
{
    public interface IWriteMessage
    {
        public Result WriteMessage(BotEventArgs sender, string message);
    }
}