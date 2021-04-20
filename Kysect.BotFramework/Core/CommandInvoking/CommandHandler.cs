using System;
using FluentResults;
using Kysect.BotFramework.Core.BotMessages;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandHandler
    {
        private readonly CommandHolder _commands = new CommandHolder();
        private readonly ServiceProvider _serviceProvider;

        public CommandHandler(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Result<CommandArgumentContainer> IsCorrectArgumentCount(CommandArgumentContainer args)
        {
            Result<BotCommandDescriptor> commandTask = _commands.GetCommand(args.CommandName);
            if (commandTask.IsFailed)
                return commandTask.ToResult<CommandArgumentContainer>();

            return commandTask.Value.Args.Length == args.Arguments.Count
                ? Result.Ok(args)
                : Result.Fail<CommandArgumentContainer>("Cannot execute command. Argument count miss matched with command signature");
        }


        public Result<CommandArgumentContainer> IsCommandCanBeExecuted(CommandArgumentContainer args)
        {
            Result<BotCommandDescriptor> commandTask = _commands.GetCommand(args.CommandName);

            if (commandTask.IsFailed)
                return commandTask.ToResult<CommandArgumentContainer>();

            IBotCommand command = commandTask.Value.ResolveCommand(_serviceProvider);

            Result canExecute = command.CanExecute(args);

            return canExecute.IsSuccess
                ? Result.Ok(args)
                : Result.Fail<CommandArgumentContainer>($"Command [{commandTask.Value.CommandName}] cannot be executed: {canExecute}");
        }

        public CommandHandler SetCaseSensitive(bool caseSensitive)
        {
            _commands.SetCaseSensitive(caseSensitive);
            return this;
        }

        public void RegisterCommand(BotCommandDescriptor descriptor)
        {
            _commands.AddCommand(descriptor);
        }

        public Result<IBotMessage> ExecuteCommand(CommandArgumentContainer args)
        {
            Result<BotCommandDescriptor> commandDescriptor = _commands.GetCommand(args.CommandName);

            if (!commandDescriptor.IsSuccess)
                return commandDescriptor.ToResult<IBotMessage>();

            try
            {
                IBotCommand command = commandDescriptor.Value.ResolveCommand(_serviceProvider);


                if (command is IBotAsyncCommand asyncCommand)
                {
                    return asyncCommand.Execute(args).Result;
                }
                if (command is IBotSyncCommand syncCommand)
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