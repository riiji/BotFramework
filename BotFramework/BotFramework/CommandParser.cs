using System.Collections.Generic;
using System.Linq;
using BotFramework.BotFramework.Exceptions;
using BotFramework.Common;

namespace BotFramework.BotFramework
{
    public class CommandParser : ICommandParser
    {
        public CommandArgumentContainer ParseCommand(BotEventArgs botArguments)
        {
            string[] commands = botArguments.Text.Split();
            var commandName = commands.FirstOrDefault() ?? throw new BotValidException("ParseCommand: commandName was null");
            
            //skip command name
            IEnumerable<string> args = commands.Skip(1);

            return new CommandArgumentContainer(commandName, new SenderData(botArguments.GroupId), args.ToList());
        }
    }
}