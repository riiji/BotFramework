using FluentResults;
using Kysect.BotFramework.Core.Commands;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public interface ICommandParser
    {
        Result<CommandContainer> ParseCommand(BotEventArgs botArguments);
    }
}