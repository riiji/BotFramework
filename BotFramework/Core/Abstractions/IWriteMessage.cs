using FluentResults;

namespace Tef.BotFramework.Core.Abstractions
{
    public interface IWriteMessage
    {
        public Result WriteMessage(BotEventArgs sender, string message);
    }
}