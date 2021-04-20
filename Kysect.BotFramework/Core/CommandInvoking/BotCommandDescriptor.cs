using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public abstract class BotCommandDescriptor
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

        public abstract IBotCommand ResolveCommand(ServiceProvider serviceProvider);
    }

    public class BotCommandDescriptor<T> : BotCommandDescriptor where T : IBotCommand
    {
        public BotCommandDescriptor(string commandName, string description, string[] args) : base(commandName, description, args)
        {
        }

        public BotCommandDescriptor(string commandName, string description) : this(commandName, description, Array.Empty<string>())
        {
        }

        public override IBotCommand ResolveCommand(ServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<T>();
        }
    }
}