using FluentResults;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public interface ICommandParser
    {
        Result<CommandContainer> ParseCommand(BotEventArgs botArguments);
    }
}