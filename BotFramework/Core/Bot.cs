using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Serilog;
using Tef.BotFramework.Abstractions;
using Tef.BotFramework.Core.CommandControllers;
using Tef.BotFramework.Tools.Loggers;

namespace Tef.BotFramework.Core
{
    public class Bot : IDisposable
    {
        private readonly CommandHandler _commandHandler;
        private readonly IBotApiProvider _botProvider;
        private readonly ICommandParser _commandParser;

        private char _prefix = '\0';
        private bool _caseSensitive = true;

        public Bot(IBotApiProvider botProvider)
        {
            _botProvider = botProvider;

            _commandParser = new CommandParser();
            _commandHandler = new CommandHandler();
        }

        public void Start()
        {
            _botProvider.OnMessage += ApiProviderOnMessage;
        }

        public Bot AddDefaultLogger()
        {
            LoggerHolder.Instance.Verbose("Initialized");
            return this;
        }

        public Bot AddLogger(ILogger logger)
        {
            LoggerHolder.Init(logger);
            LoggerHolder.Instance.Verbose("Initialized");
            return this;
        }

        public Bot SetPrefix(char prefix)
        {
            _prefix = prefix;
            return this;
        }

        public Bot WithoutCaseSensitiveCommands()
        {
            _caseSensitive = false;
            _commandHandler.WithoutCaseSensitiveCommands();
            return this;
        }

        public Bot AddCommand(IBotCommand command)
        {
            _commandHandler.RegisterCommand(command);
            return this;
        }

        public Bot AddCommands(IEnumerable<IBotCommand> commands)
        {
            foreach (var command in commands)
                _commandHandler.RegisterCommand(command);
            return this;
        }

        private void ApiProviderOnMessage(object sender, BotEventArgs e)
        {
            try
            {
                CommandArgumentContainer commandWithArgs = _commandParser.ParseCommand(e);
                var commandName = commandWithArgs.CommandName;

                if (commandName.FirstOrDefault() != _prefix && _prefix != '\0')
                    return;

                if (commandName.FirstOrDefault() == _prefix)
                    commandName = commandName.Remove(0, 1);

                if (!_caseSensitive)
                    commandName = commandName.ToLower();

                commandWithArgs =
                    new CommandArgumentContainer(commandName, commandWithArgs.Sender, commandWithArgs.Arguments);

                Result commandTaskResult = _commandHandler.IsCommandCorrect(commandWithArgs);
                LoggerHolder.Instance.Verbose($"IsCommandCorrect: [Args: {commandWithArgs}] [Result: {commandTaskResult}]");

                if (!commandTaskResult.IsSuccess)
                    return;

                Result<string> commandExecuteResult = _commandHandler.ExecuteCommand(commandWithArgs).Result;
                if (!commandExecuteResult.IsSuccess)
                    LoggerHolder.Instance.Warning($"Execute result: [Result: {commandExecuteResult}]");

                Result<string> writeMessageResult =
                    _botProvider.WriteMessage(new BotEventArgs(commandExecuteResult.Value, commandWithArgs.Sender.GroupId, commandWithArgs.Sender.UserSenderId, commandWithArgs.Sender.Username));

                LoggerHolder.Instance.Verbose($"Send message result: [Result {writeMessageResult}]");
            }
            catch (Exception exception)
            {
                LoggerHolder.Instance.Error(exception, "Message handling failed");
                _botProvider.Restart();
            }
        }

        public void Dispose()
        {
            _botProvider.OnMessage -= ApiProviderOnMessage;
        }
    }
}