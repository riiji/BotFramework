using System;
using System.Collections.Generic;
using Kysect.BotFramework.Core.CommandInvoking;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.Core.Commands
{
    public abstract class BotCommandDescriptor
    {
        public string CommandName { get; }
        public string Description { get; }
        public List<string> Args { get; }

        public BotCommandDescriptor(string commandName, string description, List<string> args)
        {
            CommandName = commandName;
            Description = description;
            Args = args;
        }

        public abstract IBotCommand ResolveCommand(ServiceProvider serviceProvider);
    }

    public class BotCommandDescriptor<T> : BotCommandDescriptor where T : IBotCommand
    {
        public BotCommandDescriptor(string commandName, string description, List<string> args) 
            : base(commandName, description, args)
        {
        }

        public BotCommandDescriptor(string commandName, string description) 
            : this(commandName, description, new List<string>())
        {
        }

        public override IBotCommand ResolveCommand(ServiceProvider serviceProvider) => serviceProvider.GetService<T>();
    }
}