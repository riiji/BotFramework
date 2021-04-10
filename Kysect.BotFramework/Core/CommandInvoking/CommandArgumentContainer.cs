using System.Collections.Generic;
using System.Linq;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandArgumentContainer
    {
        public string CommandName { get; private set; }

        public SenderInfo Sender { get; }

        public List<string> Arguments { get; }

        public CommandArgumentContainer(string commandName, SenderInfo sender, List<string> arguments)
        {
            CommandName = commandName;
            Sender = sender;
            Arguments = arguments;
        }

        public bool EnsureStartWithPrefix(char prefix)
        {
            return prefix == '\0' || CommandName.FirstOrDefault() == prefix;
        }

        public CommandArgumentContainer ApplySettings(char prefix, bool caseSensitive)
        {
            if (!caseSensitive)
                CommandName = CommandName.ToLower();

            if (CommandName.FirstOrDefault() == prefix)
                CommandName = CommandName.Remove(0, 1);

            return this;
        }

        public override string ToString()
        {
            return $"[CommandArgumentContainer CommandName:{CommandName}; Arguments:{string.Join(",", Arguments)}]";
        }
    }
}