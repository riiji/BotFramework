using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.DefaultCommands;
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
        private readonly object _lock = new object();
        private readonly TelegramSettings _settings;
        private TelegramBotClient _client;

        public TelegramApiProvider(ISettingsProvider<TelegramSettings> settingsProvider)
        {
            _settings = settingsProvider.GetSettings();
            Initialize();
        }
        public event EventHandler<BotEventArgs> OnMessage;

        public void Restart()
        {
            lock (_lock)
            {
                if (_client != null)
                {
                    Dispose();
                }

                Initialize();
            }
        }
        
        private void Initialize()
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

        private void ClientOnMessage(object sender, MessageEventArgs e)
        {
            LoggerHolder.Instance.Debug("New message event: {@e}", e);
            IBotMessage message = new BotTextMessage(string.Empty);
            string text = e.Message.Text ?? e.Message.Caption;
            switch (e.Message.Type)
            {
                case MessageType.Photo:
                {
                    var mediaFile = new BotOnlinePhotoFile(GetFileLink(e.Message.Photo.Last().FileId),
                                                           e.Message.Photo.Last().FileId);
                    message = new BotSingleMediaMessage(text, mediaFile);
                    break;
                }
                case MessageType.Video:
                {
                    var mediaFile = new BotOnlineVideoFile(GetFileLink(e.Message.Video.FileId), e.Message.Video.FileId);
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
                                      e.Message.From.FirstName,
                                      CheckIsAdmin(e.Message.From.Id,e.Message.Chat.Id)
                                  )
                              ));
        }

        private string GetFileLink(string id) =>
            $"https://api.telegram.org/file/bot{_settings.AccessToken}/{_client.GetFileAsync(id).Result.FilePath}";

        private bool CheckIsAdmin(int userId, long chatId)
        {
            var chatMember = _client.GetChatMemberAsync(chatId, userId).Result;
            return chatMember.Status is ChatMemberStatus.Administrator or ChatMemberStatus.Creator;
        }

        public Result<string> SendMultipleMedia(List<IBotMediaFile> mediaFiles, string text, SenderInfo sender)
        {
            var checkResult = CheckMediaFiles(mediaFiles);
            if (checkResult.IsFailed)
                return checkResult;

            Result<string> result = CheckText(text);
            if (result.IsFailed)
            {
                return result;
            }

            List<FileStream> streams = new List<FileStream>();
            List<IAlbumInputMedia> filesToSend = collectInputMedia(mediaFiles, text, streams);

            Task<Message[]> task = _client.SendMediaGroupAsync(filesToSend, sender.GroupId);

            try
            {
                task.Wait();
                foreach (FileStream stream in streams)
                {
                    stream.Close();
                }

                return Result.Ok("Message send");
            }
            catch (Exception e)
            {
                foreach (FileStream stream in streams)
                {
                    stream.Close();
                }
                
                const string message = "Error while sending message";
                LoggerHolder.Instance.Error(e, message);

                return Result.Fail(new Error(message).CausedBy(e));
            }
        }
        
        private List<IAlbumInputMedia> collectInputMedia(List<IBotMediaFile> mediaFiles, string text,
            List<FileStream> streams)
        {
            List<IAlbumInputMedia> filesToSend = new List<IAlbumInputMedia>();
            IAlbumInputMedia fileToSend;
            if (mediaFiles.First() is IBotOnlineFile onlineFile)
            {
                fileToSend = mediaFiles.First().MediaType switch
                {
                    MediaTypeEnum.Photo => new InputMediaPhoto(onlineFile.Id) {Caption = text},
                    MediaTypeEnum.Video => new InputMediaVideo(onlineFile.Id) {Caption = text}
                };
            }
            else
            {
                streams.Add(File.Open(mediaFiles.First().Path, FileMode.Open));
                var inputMedia = new InputMedia(streams.Last(),
                                                mediaFiles.First().Path.Split(Path.DirectorySeparatorChar).Last());
                fileToSend = mediaFiles.First().MediaType switch
                {
                    MediaTypeEnum.Photo => new InputMediaPhoto(inputMedia) {Caption = text},
                    MediaTypeEnum.Video => new InputMediaVideo(inputMedia) {Caption = text}
                };
            }

            filesToSend.Add(fileToSend);

            foreach (IBotMediaFile mediaFile in mediaFiles.Skip(1))
            {
                if (mediaFile is IBotOnlineFile onlineMediaFile)
                {
                    fileToSend = mediaFile.MediaType switch
                    {
                        MediaTypeEnum.Photo => new InputMediaPhoto(onlineMediaFile.Id) {Caption = text},
                        MediaTypeEnum.Video => new InputMediaVideo(onlineMediaFile.Id) {Caption = text}
                    };
                }
                else
                {
                    streams.Add(File.Open(mediaFile.Path, FileMode.Open));
                    var inputMedia = new InputMedia(streams.Last(),
                                                    mediaFile.Path.Split(Path.DirectorySeparatorChar).Last());
                    fileToSend = mediaFile.MediaType switch
                    {
                        MediaTypeEnum.Photo => new InputMediaPhoto(inputMedia),
                        MediaTypeEnum.Video => new InputMediaVideo(inputMedia)
                    };
                }

                filesToSend.Add(fileToSend);
            }

            return filesToSend;
        }

        private Result<string> CheckMediaFiles(List<IBotMediaFile> mediaFiles)
        {
            //TODO: hack
            if (mediaFiles.Count > 10)
            {
                const string message = "Too many files provided";
                LoggerHolder.Instance.Error(message);
                return Result.Fail(message);
            }

            return Result.Ok();
        }

        public Result<string> SendMedia(IBotMediaFile mediaFile, string text, SenderInfo sender)
        {
            Result<string> result = CheckText(text);
            if (result.IsFailed)
            {
                return result;
            }

            FileStream stream = File.Open(mediaFile.Path, FileMode.Open);
            var fileToSend = new InputMedia(stream, mediaFile.Path.Split(Path.DirectorySeparatorChar).Last());
            Task<Message> task = mediaFile.MediaType switch
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

        public Result<string> SendOnlineMedia(IBotOnlineFile file, string text, SenderInfo sender)
        {
            Result<string> result = CheckText(text);
            if (result.IsFailed)
            {
                return result;
            }

            string fileIdentifier = file.Id ?? file.Path;

            Task<Message> task = file.MediaType switch
            {
                MediaTypeEnum.Photo => _client.SendPhotoAsync(sender.GroupId, fileIdentifier, text),
                MediaTypeEnum.Video => _client.SendVideoAsync(sender.GroupId, fileIdentifier, caption: text)
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

        public Result<string> SendTextMessage(string text, SenderInfo sender)
        {
            if (text.Length == 0)
            {
                LoggerHolder.Instance.Error("The message wasn't sent by the command " +
                                            $"\"{PingCommand.Descriptor.CommandName}\", the length must not be zero.");
                return Result.Ok();
            }

            return SendText(text, sender);
        }

        private Result<string> SendText(string text, SenderInfo sender)
        {
            Result<string> result = CheckText(text);
            if (result.IsFailed)
            {
                return result;
            }

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
        
        public Result<string> SendPollMessage(string text, SenderInfo sender) => throw new NotImplementedException();

        private Result<string> CheckText(string text)
        {
            if (text.Length > 4096)
            {
                string subString = text.Substring(0, 99) + "...";
                string errorMessage = "The message wasn't sent by the command " +
                                      $"\"{PingCommand.Descriptor.CommandName}\", the length is too big.";
                return Result.Fail(new Error(errorMessage).CausedBy(subString));
            }

            return Result.Ok();
        }
    }
}