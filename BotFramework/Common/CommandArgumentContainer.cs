using System.Collections.Generic;
using Tef.BotFramework.Core;

namespace Tef.BotFramework.Common
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
    }
}