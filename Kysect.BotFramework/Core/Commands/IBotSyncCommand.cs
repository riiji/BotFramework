using FluentResults;
using Kysect.BotFramework.Core.BotMessages;

namespace Kysect.BotFramework.Core.Commands
{
    public interface IBotSyncCommand : IBotCommand
    {
        Result<IBotMessage> Execute(CommandContainer args);
    }
}