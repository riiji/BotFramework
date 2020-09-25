using FluentResults;

namespace Tef.BotFramework.Core
{
    public interface ICommandParser
    {
        Result<CommandArgumentContainer> ParseCommand(BotEventArgs botArguments);
    }
}