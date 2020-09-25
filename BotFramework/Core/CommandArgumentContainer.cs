using System.Collections.Generic;
using System.Linq;

namespace Tef.BotFramework.Core
{
    public class CommandArgumentContainer
    {
        public string CommandName { get; private set; }

        public BotEventArgs Sender { get; }

        public List<string> Arguments { get; }

        public CommandArgumentContainer(string commandName, BotEventArgs sender, List<string> arguments)
        {
            CommandName = commandName;
            Sender = sender;
            Arguments = arguments;
        }

        public bool StartWithPrefix(char prefix)
        {
            return prefix == '\0' || CommandName.FirstOrDefault() != prefix;
        }

        public void ApplySettings(char prefix, bool caseSensitive)
        {
            if (!caseSensitive)
                CommandName = CommandName.ToLower();

            if (CommandName.FirstOrDefault() != prefix)
                CommandName = CommandName.Remove(0, 1);
        }

        public override string ToString()
        {
            return $"[CommandArgumentContainer CommandName:{CommandName}; Arguments:{string.Join(",", Arguments)}]";
        }
    }
}