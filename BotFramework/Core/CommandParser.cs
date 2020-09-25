using System.Collections.Generic;
using System.Linq;
using FluentResults;

namespace Tef.BotFramework.Core
{
    public class CommandParser : ICommandParser
    {
        public Result<CommandArgumentContainer> ParseCommand(BotEventArgs botArguments)
        {
            string[] commands = botArguments.Text.Split();
            string commandName = commands.FirstOrDefault();

            if (commandName is null)
                return Result.Fail("ParseCommand: commandName was null");

            //skip command name
            IEnumerable<string> args = commands.Skip(1);

            return Result.Ok(new CommandArgumentContainer(commandName, botArguments, args.ToList()));
        }
    }
}