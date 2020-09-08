using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Tef.BotFramework.Abstractions;
using Tef.BotFramework.Common;

namespace Tef.BotFramework.Core.CommandControllers
{
    public class CommandHandler
    {
        private readonly CommandsList _commands = new CommandsList();

        public Result IsCommandCorrect(CommandArgumentContainer args)
        {
            var commandTask = _commands.GetCommand(args.CommandName);

            if (!commandTask.IsSuccess)
                return commandTask;

            var command = commandTask.Value;

            if (command.CanExecute(args))
                return Result.Ok(
                    $"command {args.CommandName} can be executable with args {string.Join(' ', args.Arguments.Select(x => x))}");

            var loggerMessage =
                $"command {command.CommandName} not executable with args {string.Join(' ', args.Arguments.Select(x => x))}";
            
            return Result.Fail(loggerMessage);
        }

        public CommandHandler WithoutCaseSensitiveCommands()
        {
            _commands.WithoutCaseSensitive();
            return this;
        }

        public void RegisterCommand(IBotCommand command)
        {
            _commands.AddCommand(command);
        }

        public async Task<Result<string>> ExecuteCommand(CommandArgumentContainer args)
        {
            Result<IBotCommand> command = _commands.GetCommand(args.CommandName);

            if (!command.IsSuccess)
                return command.ToResult<string>();

            Result<string> commandExecuteResult = await command.Value.ExecuteAsync(args);
            return commandExecuteResult;
        }

        public CommandsList GetCommands()
        {
            return _commands;
        }
    }
}