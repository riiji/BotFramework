using System.Collections.Generic;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.CommandInvoking;
using Kysect.BotFramework.Core.Tools.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Kysect.BotFramework.Core
{
    public class BotManagerBuilder
    {
        public ServiceCollection ServiceCollection { get; } = new ServiceCollection();

        private char _prefix = '\0';
        private bool _caseSensitive = true;
        private bool _sendErrorLogToUser;
        private readonly List<BotCommandDescriptor> _commands = new List<BotCommandDescriptor>();

        public BotManagerBuilder AddLogger(ILogger logger)
        {
            LoggerHolder.Init(logger);
            LoggerHolder.Instance.Information("Logger was initalized");

            return this;
        }

        public BotManagerBuilder SetPrefix(char prefix)
        {
            _prefix = prefix;
            LoggerHolder.Instance.Debug($"New prefix set: {prefix}");
            return this;
        }

        public BotManagerBuilder SetCaseSensitive(bool caseSensitive)
        {
            _caseSensitive = caseSensitive;
            LoggerHolder.Instance.Debug($"Case sensitive was updated: {caseSensitive}");

            return this;
        }

        public BotManagerBuilder EnableErrorLogToUser()
        {
            _sendErrorLogToUser = true;
            LoggerHolder.Instance.Information("Enable log redirection to user");

            return this;
        }

        public BotManagerBuilder AddCommand<T>(BotCommandDescriptor<T> descriptor) where T : class, IBotCommand
        {
            _commands.Add(descriptor);
            ServiceCollection.AddScoped<T>();
            LoggerHolder.Instance.Information($"New command added: {descriptor.CommandName}");

            return this;
        }

        public BotManager Build(IBotApiProvider apiProvider)
        {
            ServiceProvider serviceProvider = ServiceCollection.BuildServiceProvider();
            var commandHandler = new CommandHandler(serviceProvider);
            _commands.ForEach(commandHandler.RegisterCommand);
            commandHandler.SetCaseSensitive(_caseSensitive);
            return new BotManager(apiProvider, commandHandler, _prefix, _sendErrorLogToUser);
        }
    }
}