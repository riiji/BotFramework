using FluentResults;

namespace Kysect.BotFramework.Core.Commands
{
    public interface IBotCommand
    {
        Result CanExecute(CommandContainer args);
    }
}