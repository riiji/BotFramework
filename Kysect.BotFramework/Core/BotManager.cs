using System;
using FluentResults;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.CommandInvoking;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.Core
{
    public class BotManager : IDisposable
    {
        private readonly IBotApiProvider _apiProvider;
        private readonly CommandHandler _commandHandler;
        private readonly ICommandParser _commandParser;
        private readonly char _prefix;
        private readonly bool _sendErrorLogToUser;
        private readonly ServiceProvider _serviceProvider;

        public BotManager(IBotApiProvider apiProvider, CommandHandler commandHandler, char prefix,
            bool sendErrorLogToUser, ServiceProvider provider)
        {
            _serviceProvider = provider;
            _apiProvider = apiProvider;
            _prefix = prefix;
            _sendErrorLogToUser = sendErrorLogToUser;
            _commandParser = new CommandParser();
            _commandHandler = commandHandler;
        }

        public void Dispose()
        {
            _apiProvider.OnMessage -= ApiProviderOnMessage;
        }

        public void Start()
        {
            _apiProvider.OnMessage += ApiProviderOnMessage;
        }

        private void ApiProviderOnMessage(object sender, BotNewMessageEventArgs e)
        {
            try
            {
                ProcessMessage(e);
            }
            catch (Exception exception)
            {
                LoggerHolder.Instance.Error(exception, $"Message handling from [{e.SenderInfo.UserSenderUsername}] failed.");
                LoggerHolder.Instance.Debug($"Failed message: {e.Message.Text}");
                //FYI: we do not need to restart on each exception, but probably we have case were restart must be.
                //_apiProvider.Restart();
            }
        }

        private void ProcessMessage(BotNewMessageEventArgs e)
        {
            var dbContext =  _serviceProvider.GetService<BotFrameworkDbContext>();
            DialogContext context = e.SenderInfo.GetOrCreateDialogContext(dbContext);
            var botEventArgs = new BotEventArgs(e.Message, context);
            
            Result<CommandContainer> commandResult = _commandParser.ParseCommand(botEventArgs);
            if (commandResult.IsFailed)
            {
                return;
            }

            if (!commandResult.Value.StartsWithPrefix(_prefix))
            {
                return;
            }

            CommandContainer command = commandResult.Value.RemovePrefix(_prefix);

            Result checkResult = _commandHandler.CheckArgsCount(command);
            if (checkResult.IsFailed)
            {
                HandlerError(checkResult, botEventArgs);
                return;
            }

            checkResult = _commandHandler.CanCommandBeExecuted(commandResult.Value);
            if (checkResult.IsFailed)
            {
                HandlerError(checkResult, botEventArgs);
                return;
            }

            Result<IBotMessage> executionResult = _commandHandler.ExecuteCommand(commandResult.Value);
            if (executionResult.IsFailed)
            {
                HandlerError(executionResult, botEventArgs);
                return;
            }
            
            commandResult.Value.Context.Update(dbContext);

            IBotMessage message = executionResult.Value;
            SenderInfo sender = commandResult.Value.Context.SenderInfo;

            message.Send(_apiProvider, sender);
        }

        private void HandlerError(Result result, BotEventArgs botEventArgs)
        {
            LoggerHolder.Instance.Error(result.ToString());
            var errorMessage = new BotTextMessage("Something went wrong.");
            errorMessage.Send(_apiProvider, botEventArgs.Context.SenderInfo);

            if (_sendErrorLogToUser)
            {
                var errorLogMessage = new BotTextMessage(result.ToString());
                errorLogMessage.Send(_apiProvider, botEventArgs.Context.SenderInfo);
            }
        }
    }
}