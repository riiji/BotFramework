using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentResults;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Settings;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace Kysect.BotFramework.ApiProviders.Telegram
{
    public class TelegramApiProvider : IBotApiProvider, IDisposable
    {
        private readonly object _lock = new();
        private readonly TelegramSettings _settings;
        private TelegramBotClient _client;

        public TelegramApiProvider(ISettingsProvider<TelegramSettings> settingsProvider)
        {
            _settings = settingsProvider.GetSettings();
            Initialize();
        }

        public event EventHandler<BotEventArgs> OnMessage;

        public Result<string> SendText(string text, SenderInfo sender)
        {
            var task = _client.SendTextMessageAsync(sender.GroupId, text);

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

        public Result<string> SendMedia(IBotMediaFile mediaFile, string text, SenderInfo sender)
        {
            var stream = File.Open(mediaFile.Path, FileMode.Open);
            var fileToSend = new InputMedia(stream, mediaFile.Path.Split(Path.DirectorySeparatorChar).Last());
            var task = mediaFile.MediaType switch
            {
                MediaTypeEnum.Photo => _client.SendPhotoAsync(sender.GroupId, fileToSend, text),
                MediaTypeEnum.Video => _client.SendVideoAsync(sender.GroupId, fileToSend, caption: text)
            };
            
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

        public Result<string> SendMultipleMedia(List<IBotMediaFile> mediaFiles, string text, SenderInfo sender)
        {
            //todo: hack
            if (mediaFiles.Count > 10)
            {
                const string message = "Too many files provided";
                LoggerHolder.Instance.Error(message);
                return Result.Fail(new Error(message));
            }

            var streams = new List<FileStream>();
            var filesToSend = new List<IAlbumInputMedia>();

            streams.Add(File.Open(mediaFiles.First().Path, FileMode.Open));
            var inputMedia = new InputMedia(streams.Last(),
                mediaFiles.First().Path.Split(Path.DirectorySeparatorChar).Last());
            IAlbumInputMedia fileToSend = mediaFiles.First().MediaType switch
            {
                MediaTypeEnum.Photo => new InputMediaPhoto(inputMedia) {Caption = text},
                MediaTypeEnum.Video => new InputMediaVideo(inputMedia) {Caption = text}
            };
            filesToSend.Add(fileToSend);

            foreach (var mediaFile in mediaFiles.Skip(1))
            {
                streams.Add(File.Open(mediaFile.Path, FileMode.Open));
                inputMedia = new InputMedia(streams.Last(), mediaFile.Path.Split(Path.DirectorySeparatorChar).Last());
                fileToSend = mediaFile.MediaType switch
                {
                    MediaTypeEnum.Photo => new InputMediaPhoto(inputMedia),
                    MediaTypeEnum.Video => new InputMediaVideo(inputMedia)
                };
                filesToSend.Add(fileToSend);
            }

            var task = _client.SendMediaGroupAsync(filesToSend, sender.GroupId);

            try
            {
                task.Wait();
                foreach (var stream in streams) stream.Close();
                return Result.Ok("Message send");
            }
            catch (Exception e)
            {
                const string message = "Error while sending message";
                LoggerHolder.Instance.Error(e, message);
                foreach (var stream in streams) stream.Close();
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

        private void Initialize()
        {
            _client = new TelegramBotClient(_settings.AccessToken);

            _client.OnMessage += ClientOnMessage;
            _client.StartReceiving();
        }

        private void ClientOnMessage(object sender, MessageEventArgs e)
        {
            //TODO: Hm. Do we need to use try/catch here?
            Console.WriteLine("OLO");
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
    }
}