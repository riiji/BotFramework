using FluentResults;

namespace Kysect.BotFramework.Core
{
    public interface ICommandParser
    {
        Result<CommandArgumentContainer> ParseCommand(BotEventArgs botArguments);
    }
}