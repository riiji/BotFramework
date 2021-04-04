using System.Threading.Tasks;
using FluentResults;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public interface IBotCommand
    {
        string CommandName { get; }

        string Description { get; }

        string[] Args { get; }

        Result CanExecute(CommandArgumentContainer args);

        Task<Result<string>> ExecuteAsync(CommandArgumentContainer args);
    }
}