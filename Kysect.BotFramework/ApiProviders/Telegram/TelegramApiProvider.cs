using System;
using System.Threading.Tasks;
using FluentResults;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.Abstractions;
using Kysect.BotFramework.Settings;
using Kysect.BotFramework.Tools.Loggers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace Kysect.BotFramework.ApiProviders.Telegram
{
    public class TelegramApiProvider : IBotApiProvider, IDisposable
    {
        private readonly object _lock = new object();
        private TelegramBotClient _client;
        private readonly TelegramSettings _settings;

        public event EventHandler<BotEventArgs> OnMessage;

        public TelegramApiProvider(ISettingsProvider<TelegramSettings> settingsProvider)
        {
            _settings = settingsProvider.GetSettings();
            Initialize();
        }

        private void Initialize()
        {
            _client = new TelegramBotClient(_settings.AccessToken);

            _client.OnMessage += ClientOnMessage;
            _client.StartReceiving();
        }

        private void ClientOnMessage(object sender, MessageEventArgs e)
        {
            //TODO: Hm. Do we need to use try/catch here?
            LoggerHolder.Instance.Debug("New message event: {@e}", e);
            OnMessage?.Invoke(sender,
                new BotEventArgs(
                    e.Message.Text, 
                    e.Message.Chat.Id, 
                    e.Message.ForwardFromMessageId,
                    e.Message.From.FirstName
                ));
        }

        public Result<string> WriteMessage(BotEventArgs sender)
        {
            Task<Message> task = _client.SendTextMessageAsync(sender.GroupId, sender.Text);

            try
            {
                task.Wait();
                return Result.Ok("Message send");
            }
            catch (Exception e)
            {
                const string message = "Error while sending message";
                LoggerHolder.Instance.Error(e, message);
                return Result.Fail(new Error(message).CausedBy(e));
            }
        }

        public void Restart()
        {
            lock (_lock)
            {
                if (_client != null)
                    Dispose();

                Initialize();
            }
        }

        public void Dispose()
        {
            _client.OnMessage -= ClientOnMessage;
            _client.StopReceiving();
        }
    }
}