using System.Threading.Tasks;
using FluentResults;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandHandler
    {
        private readonly CommandHolder _commands = new CommandHolder();

        public Result<CommandArgumentContainer> IsCorrectArgumentCount(CommandArgumentContainer args)
        {
            Result<IBotCommand> commandTask = _commands.GetCommand(args.CommandName);
            if (!commandTask.IsSuccess)
                return commandTask.ToResult<CommandArgumentContainer>();

            return commandTask.Value.Args.Length == args.Arguments.Count
                ? Result.Ok(args)
                : Result.Fail<CommandArgumentContainer>("Cannot execute command. Argument count miss matched with command signature");
        }


        public Result<CommandArgumentContainer> IsCommandCorrect(CommandArgumentContainer args)
        {
            Result<IBotCommand> commandTask = _commands.GetCommand(args.CommandName);

            if (!commandTask.IsSuccess)
                return commandTask.ToResult<CommandArgumentContainer>();

            IBotCommand command = commandTask.Value;

            Result canExecute = command.CanExecute(args);

            return canExecute.IsSuccess
                ? Result.Ok(args)
                : Result.Fail<CommandArgumentContainer>($"Command [{command.CommandName}] cannot be executed: {canExecute}");
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

        public CommandHolder GetCommands()
        {
            return _commands;
        }
    }
}