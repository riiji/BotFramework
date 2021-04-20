using FluentResults;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public interface IBotCommand
    {
        Result CanExecute(CommandArgumentContainer args);
    }
}