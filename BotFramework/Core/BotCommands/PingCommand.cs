using System.Threading.Tasks;
using FluentResults;
using Tef.BotFramework.Core.Abstractions;

namespace Tef.BotFramework.Core.BotCommands
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

        public Task<Result<string>> ExecuteAsync(CommandArgumentContainer args)
        {
            return Task.FromResult(Result.Ok($"Pong {args.Sender.Username}"));
        }
    }
}