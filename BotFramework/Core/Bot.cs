using System;
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
        private readonly IBotApiProvider _botProvider;
        private readonly ICommandParser _commandParser;
        private char _prefix = '!';
        
        public Bot(IBotApiProvider botProvider, CommandsList commands)
        {
            _botProvider = botProvider;

            _commandParser = new CommandParser();
            _commandHandler = new CommandHandler(commands);
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
        
        private void ApiProviderOnMessage(object sender, BotEventArgs e)
        {
            try
            {
                var commandWithArgs = _commandParser.ParseCommand(e);
                
                if (commandWithArgs.CommandName.FirstOrDefault() != _prefix)
                    return;
                
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