using System.Threading.Tasks;
using Tef.BotFramework.Abstractions;
using Tef.BotFramework.Common;

namespace Tef.BotFramework.BotCommands
{
    public class PingCommand : IBotCommand
    {
        public string CommandName { get; } = "Ping";
        public string Description { get; } = "Answer pong on ping message";
        public string[] Args { get; } = new string[0];

        public bool CanExecute(CommandArgumentContainer args)
        {
            return true;
        }

        public Task<Result<string>> ExecuteAsync(CommandArgumentContainer args)
        {
            return Task.FromResult(new Result<string>(true, $"Pong {args.Sender.Username}"));
        }
    }
}