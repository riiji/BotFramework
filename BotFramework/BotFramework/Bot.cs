using System;
using Tef.BotFramework.Abstractions;
using Tef.BotFramework.BotFramework.CommandControllers;
using Tef.BotFramework.Common;
using Tef.BotFramework.Tools.Loggers;

namespace Tef.BotFramework.BotFramework
{
    public class Bot : IDisposable
    {
        private readonly CommandHandler _commandHandler;
        private readonly IBotApiProvider _botProvider;
        private readonly ICommandParser _commandParser;
        private readonly char _prefix = '!';

        public Bot(IBotApiProvider botProvider, ICommandParser commandParser, CommandsList commands)
        {
            _botProvider = botProvider;
            _commandParser = commandParser;

            _commandHandler = new CommandHandler(new CommandsList());
        }

        public void Process()
        {
            _botProvider.OnMessage += ApiProviderOnMessage;
        }

        private void ApiProviderOnMessage(object sender, BotEventArgs e)
        {
            try
            {
                var commandWithArgs = _commandParser.ParseCommand(e);
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
                _botProvider.Dispose();
                var result = _botProvider.Initialize();
                if (result.Exception != null)
                    LoggerHolder.Log.Verbose(result.ExecuteMessage);

                LoggerHolder.Log.Verbose(result.ExecuteMessage);
            }
        }

        public void Dispose()
        {
            _botProvider.OnMessage -= ApiProviderOnMessage;
        }
    }
}