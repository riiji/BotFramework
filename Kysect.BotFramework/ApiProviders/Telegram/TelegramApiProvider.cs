using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Kysect.BotFramework.Commands;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Settings;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
            const string message = "Error while sending message";
            switch (text.Length)
            {
                case (0):
                {
                    LoggerHolder.Instance.Error($"The message wasn't sent by the command " +
                                                $"\"{PingCommand.Descriptor.CommandName}\", the length must not be zero.");
                    return Result.Fail(new Error(message).CausedBy(text));
                }
                case ( > 4096):
                {
                    string subString = text.Substring(0, 99) + "...";
                    LoggerHolder.Instance.Error($"The message wasn't sent by the command " +
                                                $"\"{PingCommand.Descriptor.CommandName}\", the length is too big: {text}");
                    return Result.Fail(new Error(message).CausedBy(subString));
                }
            }

            var task = _client.SendTextMessageAsync(sender.GroupId, text);
            
            try
            {
                task.Wait();
                return Result.Ok("Message send");
            }
            catch (Exception e)
            {
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

        public Result<string> SendOnlineMedia(IBotOnlineFile file, string text, SenderInfo sender)
        {
            string fileIdentefier = file.Path;
            if (file.Id is not null)
            {
                fileIdentefier = file.Id;
            }

            Task<Message> task = file.MediaType switch
            {
                MediaTypeEnum.Photo => _client.SendPhotoAsync(sender.GroupId, fileIdentefier, text),
                MediaTypeEnum.Video => _client.SendVideoAsync(sender.GroupId, fileIdentefier, caption: text)
            };
            
            try
            {
                task.Wait();
                return Result.Ok("Message sent.");
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
            IBotMessage message = new BotTextMessage(String.Empty);
            string text = e.Message.Text is null ? e.Message.Caption : e.Message.Text;
            switch (e.Message.Type)
            {
                case MessageType.Photo:
                {
                    var mediaFile = new BotOnlinePhotoFile(GetFileLink(e.Message.Photo.Last().FileId),e.Message.Photo.Last().FileId);
                    message = new BotSingleMediaMessage(text, mediaFile);
                    break;
                }
                case MessageType.Video:
                {
                    var mediaFile = new BotOnlineVideoFile(GetFileLink(e.Message.Video.FileId),e.Message.Video.FileId);
                    message = new BotSingleMediaMessage(text, mediaFile);
                    break;
                }
                default:
                    message = new BotTextMessage(text);
                    break;
            }
            OnMessage?.Invoke(sender,
                new BotEventArgs(
                    message,
                    new SenderInfo(
                        e.Message.Chat.Id,
                        e.Message.From.Id,
                        e.Message.From.FirstName
                    )
                ));
        }

        private string GetFileLink(string id)
        {
            return $"https://api.telegram.org/file/bot{_settings.AccessToken}/{_client.GetFileAsync(id).Result.FilePath}";
        } 
    }
}
