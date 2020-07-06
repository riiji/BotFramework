using Tef.BotFramework.Common;

namespace Tef.BotFramework.Abstractions
{
    public interface IWriteMessage
    {
        public Result WriteMessage(SenderData sender, string message);
    }
}