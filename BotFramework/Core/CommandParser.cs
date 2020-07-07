using System.Collections.Generic;
using System.Linq;
using Tef.BotFramework.Common;
using Tef.BotFramework.Core.Exceptions;

namespace Tef.BotFramework.Core
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