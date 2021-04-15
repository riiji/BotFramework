using System;
using FluentResults;
using Kysect.BotFramework.Core.BotMessages;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandHandler
    {
        private readonly CommandHolder _commands = new CommandHolder();

        public Result<CommandArgumentContainer> IsCorrectArgumentCount(CommandArgumentContainer args)
        {
            Result<IBotCommand> commandTask = _commands.GetCommand(args.CommandName);
            if (commandTask.IsFailed)
                return commandTask.ToResult<CommandArgumentContainer>();

            return commandTask.Value.Args.Length == args.Arguments.Count
                ? Result.Ok(args)
                : Result.Fail<CommandArgumentContainer>("Cannot execute command. Argument count miss matched with command signature");
        }


        public Result<CommandArgumentContainer> IsCommandCanBeExecuted(CommandArgumentContainer args)
        {
            Result<IBotCommand> commandTask = _commands.GetCommand(args.CommandName);

            if (commandTask.IsFailed)
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

        public Result<IBotMessage> ExecuteCommand(CommandArgumentContainer args)
        {
            Result<IBotCommand> command = _commands.GetCommand(args.CommandName);

            if (!command.IsSuccess)
                return command.ToResult<IBotMessage>();

            try
            {
                if (command.Value is IBotAsyncCommand asyncCommand)
                {
                    return asyncCommand.Execute(args).Result;
                }
                if (command.Value is IBotSyncCommand syncCommand)
                {
                    return syncCommand.Execute(args);
                }

                return Result.Fail(new Error($"Command execution failed. Wrong command inheritance."));
            }
            catch (Exception e)
            {
                return Result.Fail(new Error($"Command execution failed. Command: {args.CommandName}; arguments: {string.Join(", ", args.Arguments)}").CausedBy(e));
            }
        }

        public CommandHolder GetCommands()
        {
            return _commands;
        }
    }
}