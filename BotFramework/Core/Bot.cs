using System;
using System.Collections.Generic;
using System.Linq;
using Tef.BotFramework.Abstractions;
using Tef.BotFramework.Common;
using Tef.BotFramework.Core.CommandControllers;
using Tef.BotFramework.Tools.Loggers;

namespace Tef.BotFramework.Core
{
    public class Bot : IDisposable
    {
        private readonly CommandHandler _commandHandler;
        private readonly CommandsList _commands;
        private readonly IBotApiProvider _botProvider;
        private readonly ICommandParser _commandParser;
        private char _prefix = '\0';

        public Bot(IBotApiProvider botProvider)
        {
            _botProvider = botProvider;

            _commandParser = new CommandParser();
            _commands = new CommandsList();
            _commandHandler = new CommandHandler(_commands);
        }

        public void Start()
        {
            _botProvider.OnMessage += ApiProviderOnMessage;
        }

        public Bot AddLogger()
        {
            LoggerHolder.Log.Verbose("Initialized");
            return this;
        }

        public Bot SetPrefix(char prefix)
        {
            _prefix = prefix;
            return this;
        }

        public Bot AddCommand(IBotCommand command)
        {
            _commands.AddCommand(command);
            return this;
        }

        public Bot AddCommands(IEnumerable<IBotCommand> commands)
        {
            foreach (var command in commands)
                _commands.AddCommand(command);

            return this;
        }

        private void ApiProviderOnMessage(object sender, BotEventArgs e)
        {
            try
            {
                var commandWithArgs = _commandParser.ParseCommand(e);

                var commandName = commandWithArgs.CommandName;

                if (commandName.FirstOrDefault() != _prefix && _prefix != '\0')
                    return;

                if (commandName.FirstOrDefault() == _prefix)
                    commandName = commandName.Remove(0, 1);

                commandWithArgs =
                    new CommandArgumentContainer(commandName, commandWithArgs.Sender, commandWithArgs.Arguments);

                var commandTaskResult = _commandHandler.IsCommandCorrect(commandWithArgs);

                LoggerHolder.Log.Verbose(commandTaskResult.ExecuteMessage);

                if (!commandTaskResult.IsSuccess)
                    return;

                var commandExecuteResult = _commandHandler.ExecuteCommand(commandWithArgs);

                if (!commandExecuteResult.IsSuccess)
                    LoggerHolder.Log.Warning(commandExecuteResult.ExecuteMessage);

                var writeMessageResult =
                    _botProvider.WriteMessage(new SenderData(e.GroupId), commandExecuteResult.ExecuteMessage);

                LoggerHolder.Log.Verbose(writeMessageResult.ExecuteMessage);
            }
            catch (Exception error)
            {
                LoggerHolder.Log.Error(error.Message);
                _botProvider.OnFail();
            }
        }

        public void Dispose()
        {
            _botProvider.OnMessage -= ApiProviderOnMessage;
        }
    }
}