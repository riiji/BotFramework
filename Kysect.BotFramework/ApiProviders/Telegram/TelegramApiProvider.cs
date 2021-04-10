using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Settings;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using File = Telegram.Bot.Types.File;

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
                    new BotTextMessage(e.Message.Text), 
                    new SenderInfo(
                    e.Message.Chat.Id, 
                    e.Message.ForwardFromMessageId,
                    e.Message.From.FirstName
                    )
                ));
        }

        public Result<string> SendText(string text, SenderInfo sender)
        {
            Task<Message> task = _client.SendTextMessageAsync(sender.GroupId, text);

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

        public Result<string> SendImage(string imagePath, string text, SenderInfo sender)
        {
            Task<Message> task;
            var stream = System.IO.File.Open(imagePath, FileMode.Open);
            InputMedia fts = new InputMedia(stream,imagePath.Split("\\").Last());
            task = _client.SendPhotoAsync(sender.GroupId, fts, text);
            try
            {
                task.Wait();
                stream.Close();
                return Result.Ok("Message send");
            }
            catch (Exception e)
            {
                const string message = "Error while sending message";
                LoggerHolder.Instance.Error(e, message);
                stream.Close();
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