using FluentResults;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public interface ICommandParser
    {
        Result<CommandArgumentContainer> ParseCommand(BotEventArgs botArguments);
    }
}