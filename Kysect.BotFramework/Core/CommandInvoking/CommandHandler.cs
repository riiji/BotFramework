using System;
using FluentResults;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Commands;
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

        public Result CheckArgsCount(CommandContainer args)
        {
            Result<BotCommandDescriptor> commandTask = _commands.GetCommand(args.CommandName);
            if (commandTask.IsFailed)
            {
                return commandTask.ToResult<CommandContainer>();
            }

            return commandTask.Value.Args.Length == args.Arguments.Count
                ? Result.Ok()
                : Result.Fail<CommandContainer>(
                    "Cannot execute command. Argument count miss matched with command signature");
        }


        public Result CanCommandBeExecuted(CommandContainer args)
        {
            Result<BotCommandDescriptor> commandTask = _commands.GetCommand(args.CommandName);

            if (commandTask.IsFailed)
            {
                return commandTask.ToResult<CommandContainer>();
            }

            IBotCommand command = commandTask.Value.ResolveCommand(_serviceProvider);

            Result canExecute = command.CanExecute(args);

            return canExecute.IsSuccess
                ? Result.Ok()
                : Result.Fail<CommandContainer>(
                    $"Command [{commandTask.Value.CommandName}] cannot be executed: {canExecute}");
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

        public Result<IBotMessage> ExecuteCommand(CommandContainer args)
        {
            Result<BotCommandDescriptor> commandDescriptor = _commands.GetCommand(args.CommandName);

            if (!commandDescriptor.IsSuccess)
            {
                return commandDescriptor.ToResult<IBotMessage>();
            }

            try
            {
                IBotCommand command = commandDescriptor.Value.ResolveCommand(_serviceProvider);

                return command switch
                {
                    IBotAsyncCommand asyncCommand => asyncCommand.Execute(args).Result,
                    IBotSyncCommand syncCommand => syncCommand.Execute(args),
                    _ => Result.Fail(new Error("Command execution failed. Wrong command inheritance."))
                };
            }
            catch (Exception e)
            {
                string errorMessage =
                    $"Command execution failed. Command: {args.CommandName}; arguments: {string.Join(", ", args.Arguments)}";
                return Result.Fail(new Error(errorMessage).CausedBy(e));
            }
        }

        public CommandHolder GetCommands() => _commands;
    }
}