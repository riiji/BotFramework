using System.Threading.Tasks;
using FluentResults;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.CommandInvoking;
using Telegram.Bot.Types;

namespace Kysect.BotFramework.Commands
{
    public class PingCommand : IBotCommand
    {
        public string CommandName { get; } = "Ping";
        public string Description { get; } = "Answer pong on ping message";
        public string[] Args { get; } = new string[0];

        public Result CanExecute(CommandArgumentContainer args)
        {
            return Result.Ok();
        }

        public Task<Result<BotMessage>> ExecuteAsync(CommandArgumentContainer args)
        {
            return Task.FromResult(Result.Ok(new BotMessage($"Pong {args.Sender.Username}")));
        }
    }
}