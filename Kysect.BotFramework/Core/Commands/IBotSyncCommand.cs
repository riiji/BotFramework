using FluentResults;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.CommandInvoking;

namespace Kysect.BotFramework.Core.Commands
{
    public interface IBotSyncCommand : IBotCommand
    {
        Result<IBotMessage> Execute(CommandContainer args);
    }
}