using System.Collections.Generic;
using FluentResults;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandHolder
    {
        private bool _caseSensitive = true;
        private Dictionary<string, IBotCommand> _commands = new Dictionary<string, IBotCommand>();

        public CommandHolder WithoutCaseSensitive()
        {
            _caseSensitive = false;
            var newCommandList = new Dictionary<string, IBotCommand>();

            foreach ((var key, IBotCommand value) in _commands)
                newCommandList.TryAdd(key.ToLower(), value);
            
            _commands = newCommandList;
            return this;
        }

        public void AddCommand(IBotCommand command)
        {
            _commands.TryAdd(_caseSensitive ? command.CommandName : command.CommandName.ToLower(), command);
        }

        public Result<IBotCommand> GetCommand(string commandName)
        {
            return _commands.TryGetValue(commandName, out IBotCommand command)
                ? Result.Ok(command)
                : Result.Fail<IBotCommand>($"Command {commandName} not founded");
        }
    }
}