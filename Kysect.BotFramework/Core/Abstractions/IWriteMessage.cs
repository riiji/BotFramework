using FluentResults;

namespace Kysect.BotFramework.Core.Abstractions
{
    public interface IWriteMessage
    {
        public Result WriteMessage(BotEventArgs sender, string message);
    }
}