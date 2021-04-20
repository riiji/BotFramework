using System;
using System.Collections.Generic;
using FluentResults;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandHolder
    {
        private bool _caseSensitive = true;
        private readonly List<BotCommandDescriptor> _commands = new List<BotCommandDescriptor>();

        public CommandHolder WithoutCaseSensitive()
        {
            _caseSensitive = false;
            return this;
        }

        public void AddCommand<T>(BotCommandDescriptor<T> descriptor) where T : IBotCommand
        {
            _commands.Add(descriptor);
        }

        public Result<BotCommandDescriptor> GetCommand(string commandName)
        {
            foreach (BotCommandDescriptor command in _commands)
            {
                if (!_caseSensitive && string.Equals(command.CommandName, commandName, StringComparison.InvariantCultureIgnoreCase))
                    return Result.Ok(command);

                if (string.Equals(command.CommandName, commandName))
                    return Result.Ok(command);
            }

            return Result.Fail<BotCommandDescriptor>($"Command {commandName} not founded");
        }
    }
}