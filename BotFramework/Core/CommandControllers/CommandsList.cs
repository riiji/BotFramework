using System.Collections.Generic;
using Tef.BotFramework.Abstractions;
using Tef.BotFramework.Common;

namespace Tef.BotFramework.Core.CommandControllers
{
    public class CommandsList
    {
        private Dictionary<string, IBotCommand> _commands = new Dictionary<string, IBotCommand>();

        public CommandsList WithoutCaseSensitive()
        {
            var newCommandList = new Dictionary<string, IBotCommand>();

            foreach (var command in _commands)
                newCommandList.TryAdd(command.Key.ToLower(), command.Value);
            
            _commands = newCommandList;
            return this;
        }

        public void AddCommand(IBotCommand command)
        {
            _commands.TryAdd(command.CommandName, command);
        }

        public Result<IBotCommand> GetCommand(string commandName)
        {
            return _commands.TryGetValue(commandName, out IBotCommand command)
                ? new Result<IBotCommand>(true, command)
                : new Result<IBotCommand>(false, $"command {commandName} not founded", null);
        }
    }
}