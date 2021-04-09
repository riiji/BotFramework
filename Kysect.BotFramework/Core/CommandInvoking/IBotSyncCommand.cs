using FluentResults;
using Kysect.BotFramework.Core.BotMessages;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public interface IBotSyncCommand : IBotCommand
    {
        Result<IBotMessage> Execute(CommandArgumentContainer args);
    }
}