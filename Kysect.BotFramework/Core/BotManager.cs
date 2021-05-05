using System;
using FluentResults;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.CommandInvoking;
using Kysect.BotFramework.Core.Tools.Loggers;

namespace Kysect.BotFramework.Core
{
    public class BotManager : IDisposable
    {
        private readonly CommandHandler _commandHandler;
        private readonly IBotApiProvider _apiProvider;
        private readonly ICommandParser _commandParser;
        private readonly char _prefix;
        private readonly bool _sendErrorLogToUser;

        public BotManager(IBotApiProvider apiProvider, CommandHandler commandHandler, char prefix, bool sendErrorLogToUser)
        {
            _apiProvider = apiProvider;
            _prefix = prefix;
            _sendErrorLogToUser = sendErrorLogToUser;
            _commandParser = new CommandParser();
            _commandHandler = commandHandler;
        }

        public void Start()
        {
            _apiProvider.OnMessage += ApiProviderOnMessage;
        }

        private void ApiProviderOnMessage(object sender, BotEventArgs e)
        {
            
            try
            {
                ProcessMessage(e);
            }
            catch (Exception exception)
            {
                LoggerHolder.Instance.Error(exception, $"Message handling from [{e.Sender.Username}] failed.");
                LoggerHolder.Instance.Debug($"Failed message: {e.Message.Text}");
                //FYI: we do not need to restart on each exception, but probably we have case were restart must be.
                //_apiProvider.Restart();
            }
        }

        private void ProcessMessage(BotEventArgs e)
        {
            Result<CommandArgumentContainer> commandResult = _commandParser.ParseCommand(e);
            if (commandResult.IsFailed)
            {
                return;
            }

            if (!commandResult.Value.EnsureStartWithPrefix(_prefix))
                return;

            commandResult = _commandHandler.IsCorrectArgumentCount(commandResult.Value.ApplySettings(_prefix));
            if (commandResult.IsFailed)
            {
                HandlerError(commandResult, e);
                return;
            }

            commandResult = _commandHandler.IsCommandCanBeExecuted(commandResult.Value);
            if (commandResult.IsFailed)
            {
                HandlerError(commandResult, e);
                return;
            }

            Result<IBotMessage> executionResult = _commandHandler.ExecuteCommand(commandResult.Value);
            if (executionResult.IsFailed)
            {
                HandlerError(commandResult, e);
                return;
            }

            IBotMessage message = executionResult.Value;
            SenderInfo sender = commandResult.Value.Sender;
            //_apiProvider.WriteMessage(new BotEventArgs(executionResult.Value, commandResult.Value));
            message.Send(_apiProvider,sender);
        }

        private void HandlerError(Result result, BotEventArgs botEventArgs)
        {
            LoggerHolder.Instance.Error(result.ToString());
            BotTextMessage errorMessage = new BotTextMessage("Something went wrong.");
            errorMessage.Send(_apiProvider,botEventArgs.Sender);
            if (_sendErrorLogToUser)
            {
                BotTextMessage errorlogMessage = new BotTextMessage(result.ToString());
                errorlogMessage.Send(_apiProvider,botEventArgs.Sender);
            }
        }

        public void Dispose()
        {
            _apiProvider.OnMessage -= ApiProviderOnMessage;
        }
    }
}
