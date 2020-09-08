using System;
using System.Threading.Tasks;
using FluentResults;
using Tef.BotFramework.Abstractions;
using Tef.BotFramework.Core;
using Tef.BotFramework.Settings;
using Tef.BotFramework.Tools.Loggers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace Tef.BotFramework.Telegram
{
    public class TelegramApiProvider : IBotApiProvider, IDisposable
    {
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
            LoggerHolder.Instance.Debug("New message event: {@e}", e);
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
            _client = new TelegramBotClient(_settings.AccessToken);

            _client.OnMessage += ClientOnMessage;
            _client.StartReceiving();
        }

        public void Dispose()
        {
            _client.OnMessage -= ClientOnMessage;
            _client.StopReceiving();
        }
    }
}