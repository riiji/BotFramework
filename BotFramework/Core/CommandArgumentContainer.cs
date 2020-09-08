using System.Collections.Generic;

namespace Tef.BotFramework.Core
{
    public class CommandArgumentContainer
    {
        public string CommandName { get; }

        public BotEventArgs Sender { get; }

        public List<string> Arguments { get; }

        public CommandArgumentContainer(string commandName, BotEventArgs sender, List<string> arguments)
        {
            CommandName = commandName;
            Sender = sender;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"[CommandArgumentContainer CommandName:{CommandName}; Arguments:{string.Join(",", Arguments)}]";
        }
    }
}