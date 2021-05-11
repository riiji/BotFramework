using System.Collections.Generic;
using System.Linq;
using Kysect.BotFramework.Core.BotMedia;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandArgumentContainer
    {
        public string CommandName { get; private set; }

        public SenderInfo Sender { get; }

        public List<string> Arguments { get; }
        
        public List<IBotMediaFile> MediaFiles { get; }
        
        public CommandArgumentContainer(string commandName, SenderInfo sender, List<string> arguments, List<IBotMediaFile> mediaFiles)
        {
            CommandName = commandName;
            Sender = sender;
            Arguments = arguments;
            MediaFiles = mediaFiles;
        }

        public bool EnsureStartWithPrefix(char prefix)
        {
            return prefix == '\0' || CommandName.FirstOrDefault() == prefix;
        }

        public CommandArgumentContainer ApplySettings(char prefix)
        {
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