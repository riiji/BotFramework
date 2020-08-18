using System;
using Serilog;
using Serilog.Core;
using Tef.BotFramework.Abstractions;
using Tef.BotFramework.Common;
using Tef.BotFramework.Core;
using Tef.BotFramework.Settings;
using Tef.BotFramework.Tools.Extensions;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Tef.BotFramework.Telegram
{
    public class TelegramApiProvider : IBotApiProvider, IDisposable
    {
        private TelegramBotClient _client;
        private readonly TelegramSettings _settings;
        private readonly Logger _telegramLogger;

        public event EventHandler<BotEventArgs> OnMessage;

        public TelegramApiProvider(IGetSettings<TelegramSettings> settings)
        {
            _telegramLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File("telegram-api.txt")
                .CreateLogger();

            _settings = settings.GetSettings();
            _client = new TelegramBotClient(_settings.AccessToken);

            _client.OnMessage += ClientOnMessage;
            _client.StartReceiving();
        }

        private void ClientOnMessage(object sender, MessageEventArgs e)
        {
            _telegramLogger.Debug("New message event: {@e}", e);
            OnMessage?.Invoke(sender,
                new BotEventArgs(e.Message.Text, e.Message.Chat.Id, e.Message.ForwardFromMessageId, e.Message.From.FirstName));
        }

        public Result WriteMessage(BotEventArgs sender)
        {
            var result = _client.SendTextMessageAsync(sender.GroupId, sender.Text);
            result.WaitSafe();
            if (result.IsCompletedSuccessfully)
                return new Result(true, "ok");
            return new Result(false, result.Result.Text);
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