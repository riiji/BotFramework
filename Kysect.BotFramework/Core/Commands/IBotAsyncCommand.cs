using System.Threading.Tasks;
using FluentResults;
using Kysect.BotFramework.Core.BotMessages;

namespace Kysect.BotFramework.Core.Commands
{
    public interface IBotAsyncCommand : IBotCommand
    {
        Task<Result<IBotMessage>> Execute(CommandContainer args);
    }
}