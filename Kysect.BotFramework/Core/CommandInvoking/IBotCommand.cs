using System.Threading.Tasks;
using FluentResults;
using Kysect.BotFramework.Core.BotMessages;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public interface IBotCommand
    {
        string CommandName { get; }

        string Description { get; }

        string[] Args { get; }

        Result CanExecute(CommandArgumentContainer args);

        //TODO: make sync?
        Task<Result<IBotMessage>> ExecuteAsync(CommandArgumentContainer args);
    }
}