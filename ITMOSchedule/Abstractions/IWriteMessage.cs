using BotFramework.Common;

namespace BotFramework.Abstractions
{
    public interface IWriteMessage
    {
        public Result WriteMessage(SenderData sender, string message);
    }
}