using System;
using System.Threading.Tasks;
using FluentResults;
using Tef.BotFramework.Core;
using Tef.BotFramework.Core.Abstractions;
using Tef.BotFramework.Settings;
using Tef.BotFramework.Tools.Loggers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace Tef.BotFramework.Telegram
{
    public class TelegramApiProvider : IBotApiProvider, IDisposable
    {
        private readonly object _lock = new object();
        private TelegramBotClient _client;
        private readonly TelegramSettings _settings;

        public event EventHandler<BotEventArgs> OnMessage;

        public TelegramApiProvider(IGetSettings<TelegramSettings> settings)
        {
            _settings = settings.GetSettings();
            _client = new TelegramBotClient(_settings.AccessToken);

            _client.OnMessage += ClientOnMessage;
            _client.StartReceiving();
        }

        private void ClientOnMessage(object sender, MessageEventArgs e)
        {
            //TODO: Hm. Do we need to use try/catch here?
            LoggerHolder.Instance.Debug("New message event: {@e}", e);
            //TODO: line to long, wrap creating: new T(\n value,\n value2, \n...)
            OnMessage?.Invoke(sender,
                new BotEventArgs(e.Message.Text, e.Message.Chat.Id, e.Message.ForwardFromMessageId, e.Message.From.FirstName));
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

                _client = new TelegramBotClient(_settings.AccessToken);

                _client.OnMessage += ClientOnMessage;
                _client.StartReceiving();
            }
        }

        public void Dispose()
        {
            _client.OnMessage -= ClientOnMessage;
            _client.StopReceiving();
        }
    }
}