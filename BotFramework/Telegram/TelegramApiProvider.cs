using System;
using System.Threading.Tasks;
using FluentResults;
using Serilog;
using Serilog.Core;
using Tef.BotFramework.Abstractions;
using Tef.BotFramework.Core;
using Tef.BotFramework.Settings;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

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
                return Result.Fail(new Error("Error while sending message").CausedBy(e));
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