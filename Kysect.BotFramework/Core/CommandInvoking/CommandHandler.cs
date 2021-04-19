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
            var commandTask = _commands.GetCommand(args.CommandName);
            if (commandTask.IsFailed)
                return commandTask.ToResult<CommandArgumentContainer>();

            return commandTask.Value.Item1.Args.Length == args.Arguments.Count
                ? Result.Ok(args)
                : Result.Fail<CommandArgumentContainer>("Cannot execute command. Argument count miss matched with command signature");
        }


        public Result<CommandArgumentContainer> IsCommandCanBeExecuted(CommandArgumentContainer args)
        {
            var commandTask = _commands.GetCommand(args.CommandName);

            if (commandTask.IsFailed)
                return commandTask.ToResult<CommandArgumentContainer>();

            IBotCommand command = commandTask.Value.Item2;

            Result canExecute = command.CanExecute(args);

            return canExecute.IsSuccess
                ? Result.Ok(args)
                : Result.Fail<CommandArgumentContainer>($"Command [{commandTask.Value.Item1.CommandName}] cannot be executed: {canExecute}");
        }

        public CommandHandler WithoutCaseSensitiveCommands()
        {
            _commands.WithoutCaseSensitive();
            return this;
        }

        public void RegisterCommand<T>(T command, BotCommandDescriptor<T> descriptor) where T : IBotCommand
        {
            _commands.AddCommand(command, descriptor);
        }

        public Result<IBotMessage> ExecuteCommand(CommandArgumentContainer args)
        {
            var command = _commands.GetCommand(args.CommandName);

            if (!command.IsSuccess)
                return command.ToResult<IBotMessage>();

            try
            {
                if (command.Value.Item2 is IBotAsyncCommand asyncCommand)
                {
                    return asyncCommand.Execute(args).Result;
                }
                if (command.Value.Item2 is IBotSyncCommand syncCommand)
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