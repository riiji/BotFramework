using System;
using System.Collections.Generic;
using FluentResults;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandHolder
    {
        private bool _caseSensitive = true;
        private readonly List<(BotCommandDescriptor, IBotCommand)> _commands = new List<(BotCommandDescriptor, IBotCommand)>();

        public CommandHolder WithoutCaseSensitive()
        {
            _caseSensitive = false;
            return this;
        }

        public void AddCommand<T>(T command, BotCommandDescriptor<T> descriptor) where T : IBotCommand
        {
            _commands.Add((descriptor, command));
        }

        public Result<(BotCommandDescriptor, IBotCommand)> GetCommand(string commandName)
        {
            foreach ((BotCommandDescriptor, IBotCommand) tuple in _commands)
            {
                if (_caseSensitive && string.Equals(tuple.Item1.CommandName, commandName, StringComparison.InvariantCultureIgnoreCase))
                    return Result.Ok(tuple);

                if (string.Equals(tuple.Item1.CommandName, commandName))
                    return Result.Ok(tuple);
            }

            return Result.Fail<(BotCommandDescriptor, IBotCommand)>($"Command {commandName} not founded");
        }
    }
}