namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class BotCommandDescriptor
    {
        public BotCommandDescriptor(string commandName, string description, string[] args)
        {
            CommandName = commandName;
            Description = description;
            Args = args;
        }

        public string CommandName { get; }
        public string Description { get; }
        public string[] Args { get; }
    }

    public class BotCommandDescriptor<T> : BotCommandDescriptor where T : IBotCommand
    {
        public BotCommandDescriptor(string commandName, string description, string[] args) : base(commandName, description, args)
        {
        }
    }
}