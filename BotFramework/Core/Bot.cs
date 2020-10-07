using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentResults;
using Serilog;
using Tef.BotFramework.Core.Abstractions;
using Tef.BotFramework.Core.CommandControllers;
using Tef.BotFramework.Tools;
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
                ProcessMessage(e);
            }
            catch (Exception exception)
            {
                LoggerHolder.Instance.Error(exception, "Message handling failed");
                //FYI: we do not need to restart on each exception, but probably we have case were restart must be.
                //_botProvider.Restart();
            }
        }

        private void ProcessMessage(BotEventArgs e)
        {
            Result<string> result = FluentValidator
                .Init(_commandParser.ParseCommand(e))
                .Continue(arguments => arguments.EnsureStartWithPrefix(_prefix))
                .Continue(commandWithArgs => _commandHandler.IsCorrectArgumentCount(commandWithArgs.ApplySettings(_prefix, _caseSensitive)))
                .Continue(arg => _commandHandler.IsCommandCorrect(arg))
                .Continue(arg =>
                {
                    Task<Result<string>> executeTask = _commandHandler.ExecuteCommand(arg);
                    executeTask.WaitSafe();
                    if (executeTask.IsFaulted)
                        return Result.Fail<(string, CommandArgumentContainer)>(executeTask.Exception?.ToString());
                    if (executeTask.Result.IsSuccess)
                        return Result.Ok((executeTask.Result.Value, arg));
                    return executeTask.Result.ToResult<(string, CommandArgumentContainer)>();
                })
                .Continue(tuple =>
                {
                    (string value, CommandArgumentContainer commandArgumentContainer) = tuple;
                    return _botProvider.WriteMessage(new BotEventArgs(value, commandArgumentContainer));
                })
                .Value;

            if (result.IsFailed)
            {
                LoggerHolder.Instance.Error(result.ToString());
                _botProvider.WriteMessage(new BotEventArgs(result.ToString(), e));
            }
        }

        public void Dispose()
        {
            _botProvider.OnMessage -= ApiProviderOnMessage;
        }
    }
}