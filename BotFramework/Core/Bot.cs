using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentResults;
using Serilog;
using Tef.BotFramework.Core.Abstractions;
using Tef.BotFramework.Core.CommandControllers;
using Tef.BotFramework.Tools.Extensions;
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
                Result<CommandArgumentContainer> commandWithArgsResult = _commandParser.ParseCommand(e);
                if (commandWithArgsResult.IsFailed)
                {
                    var message = $"Command parse error. [Result: {commandWithArgsResult}]";
                    LoggerHolder.Instance.Error(message);
                    return;
                }

                CommandArgumentContainer commandWithArgs = commandWithArgsResult.Value;
                if (!commandWithArgs.StartWithPrefix(_prefix))
                    return;

                commandWithArgs.ApplySettings(_prefix, _caseSensitive);

                Result<bool> isCommandCorrectResult = _commandHandler.IsCommandCorrect(commandWithArgs);
                LoggerHolder.Instance.Verbose($"IsCommandCorrect: [Args: {commandWithArgs}] [Result: {isCommandCorrectResult}]");
                if (isCommandCorrectResult.IsFailed)
                {
                    var message = $"Command correct check error. [Result: {isCommandCorrectResult}]";
                    LoggerHolder.Instance.Error(message);
                    _botProvider.WriteMessage(new BotEventArgs(message, commandWithArgs));
                    return;
                }

                Task<Result<string>> executeTask = _commandHandler.ExecuteCommand(commandWithArgs);
                executeTask.WaitSafe();
                if (executeTask.IsFaulted)
                {
                    var message = $"Command execute failed. [Exception: {executeTask.Exception}]";
                    LoggerHolder.Instance.Error(message);
                    _botProvider.WriteMessage(new BotEventArgs(message, commandWithArgs));
                    return;
                }

                Result<string> commandExecuteResult = executeTask.Result;
                if (commandExecuteResult.IsFailed)
                {
                    var message = $"Execute result: [Result: {commandExecuteResult}]";
                    LoggerHolder.Instance.Error(message);
                    _botProvider.WriteMessage(new BotEventArgs(message, commandWithArgs));
                    return;
                }

                Result<string> writeMessageResult = _botProvider.WriteMessage(new BotEventArgs(commandExecuteResult.Value, commandWithArgs));
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