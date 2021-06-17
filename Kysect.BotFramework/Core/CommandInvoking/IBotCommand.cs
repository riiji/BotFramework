using FluentResults;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public interface IBotCommand
    {
        Result CanExecute(CommandContainer args);
    }
}