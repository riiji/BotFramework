using System;
using System.Collections.Generic;
using FluentResults;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandHolder
    {
        private readonly List<BotCommandDescriptor> _commands = new List<BotCommandDescriptor>();
        private bool _caseSensitive = true;

        public CommandHolder SetCaseSensitive(bool caseSensitive)
        {
            _caseSensitive = caseSensitive;
            return this;
        }

        public void AddCommand(BotCommandDescriptor descriptor)
        {
            _commands.Add(descriptor);
        }

        public Result<BotCommandDescriptor> GetCommand(string commandName)
        {
            foreach (BotCommandDescriptor command in _commands)
            {
                if (!_caseSensitive && string.Equals(command.CommandName, commandName,
                                                     StringComparison.InvariantCultureIgnoreCase))
                {
                    return Result.Ok(command);
                }

                if (string.Equals(command.CommandName, commandName))
                {
                    return Result.Ok(command);
                }
            }

            return Result.Fail<BotCommandDescriptor>($"Command {commandName} not founded");
        }
    }
}