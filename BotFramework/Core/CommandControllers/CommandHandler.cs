using System.Threading.Tasks;
using FluentResults;
using Tef.BotFramework.Core.Abstractions;

namespace Tef.BotFramework.Core.CommandControllers
{
    public class CommandHandler
    {
        private readonly CommandsList _commands = new CommandsList();

        public Result IsCommandCorrect(CommandArgumentContainer args)
        {
            Result<IBotCommand> commandTask = _commands.GetCommand(args.CommandName);

            if (!commandTask.IsSuccess)
                return commandTask.ToResult<bool>();

            IBotCommand command = commandTask.Value;

            Result<bool> canExecute = command.CanExecute(args);

            return canExecute.IsSuccess
                ? canExecute
                : Result.Fail<bool>($"Command [{command.CommandName}] can be executed: {canExecute}");
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