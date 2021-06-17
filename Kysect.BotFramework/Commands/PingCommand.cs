using System.Threading.Tasks;
using FluentResults;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.CommandInvoking;

namespace Kysect.BotFramework.Commands
{
    public class PingCommand : IBotAsyncCommand
    {
        public static readonly BotCommandDescriptor<PingCommand> Descriptor = new BotCommandDescriptor<PingCommand>(
            "Ping",
            "Answer pong on ping message");

        public Result CanExecute(CommandContainer args)
        {
            return Result.Ok();
        }

        public Task<Result<IBotMessage>> Execute(CommandContainer args)
        {
            IBotMessage message = new BotTextMessage("Pong!");
            return Task.FromResult(Result.Ok(message));
        }
    }
}