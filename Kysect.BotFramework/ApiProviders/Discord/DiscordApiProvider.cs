using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FluentResults;
using Kysect.BotFramework.Commands;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Settings;

namespace Kysect.BotFramework.ApiProviders.Discord
{
    public class DiscordApiProvider : IBotApiProvider, IDisposable
    {
        private readonly object _lock = new();
        private readonly DiscordSettings _settings;
        private DiscordSocketClient _client;
        
        public event EventHandler<BotEventArgs> OnMessage;

        public DiscordApiProvider(ISettingsProvider<DiscordSettings> settingsProvider)
        {
            _settings = settingsProvider.GetSettings();
            Initialize();
        }
        
        private void Initialize()
        {
            _client = new DiscordSocketClient();

            _client.LoginAsync(TokenType.Bot, _settings.AccessToken);

            _client.MessageReceived += ClientOnMessage;
            _client.StartAsync();
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
            _client.MessageReceived -= ClientOnMessage;
            _client.StopAsync();
        }

        private Task ClientOnMessage(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message is null) return Task.CompletedTask;
            var context = new SocketCommandContext(_client, message);
            if (context.User.IsBot || context.Guild is null) return Task.CompletedTask;
            
            LoggerHolder.Instance.Debug($"New message event: {context.Message.ToString()}");

            IBotMessage botMessage = parseMessage(message, context);

            OnMessage?.Invoke(context.Client,
                new BotEventArgs(
                    botMessage,
                    new SenderInfo(
                        (long) (context.Guild?.Id ?? 0),
                        (long) context.Channel.Id,
                        context.User.Username
                    )
                ));
            
            return Task.CompletedTask;
        }

        private IBotMessage parseMessage(SocketUserMessage message, SocketCommandContext context)
        {
            if (context.Message.Attachments.Count == 0)
                return new BotTextMessage(context.Message.Content);

            if (context.Message.Attachments.Count == 1)
            {
                var onlineFile = getOnlineFile(message.Attachments.First().Filename, message.Attachments.First().Url);
                return onlineFile is not null
                    ? new BotSingleMediaMessage(context.Message.Content, onlineFile)
                    : new BotTextMessage(context.Message.Content);
            }

            List<IBotMediaFile> mediaFiles = context.Message.Attachments
                                                    .Select(attachment => getOnlineFile(attachment.Filename, attachment.Url))
                                                    .Where(onlineFile => onlineFile is not null).Cast<IBotMediaFile>().ToList();

            if (!mediaFiles.Any())
                return new BotTextMessage(context.Message.Content);
             
            
            if (mediaFiles.Count == 1)
                return new BotSingleMediaMessage(context.Message.Content, mediaFiles.First());
            
            
            return new BotMultipleMediaMessage(context.Message.Content, mediaFiles);
            
        }

        private IBotOnlineFile getOnlineFile(string filename, string url)
        {
            switch (parseMediaType(filename))
            {
                case MediaTypeEnum.Photo: return new BotOnlinePhotoFile(url); 
                case MediaTypeEnum.Video: return new BotOnlineVideoFile(url);
                case MediaTypeEnum.Undefined:
                default: LoggerHolder.Instance.Information($"Skipped file: {filename}"); return null;
            }
        }

        private MediaTypeEnum parseMediaType(string filename)
        {
            if (filename.EndsWith("png") || filename.EndsWith("jpg") ||
                filename.EndsWith("bmp")) return MediaTypeEnum.Photo;
            if (filename.EndsWith("mp4") || filename.EndsWith("mov") ||
                filename.EndsWith("wmv") || filename.EndsWith("avi")) return MediaTypeEnum.Video;
            return MediaTypeEnum.Undefined;
        }
        
        public Result<string> SendMultipleMedia(List<IBotMediaFile> mediaFiles, string text, SenderInfo sender)
        {
            Result<string> result;
            if (mediaFiles.First() is IBotOnlineFile onlineFile)
            {
                result = SendOnlineMedia(onlineFile, text, sender);
            }
            else
            {
                result = SendMedia(mediaFiles.First(), text, sender);
            }
            
            foreach (var media in mediaFiles.Skip(1))
            {
                if (result.IsFailed) return result;
                if (media is IBotOnlineFile)
                {
                    result = SendOnlineMedia(media as IBotOnlineFile, String.Empty,  sender);
                }
                else
                {
                    result = SendMedia(media, string.Empty, sender);
                }
            }

            return result;
        }
        
        public Result<string> SendMedia(IBotMediaFile mediaFile, string text, SenderInfo sender)
        {
            var result = checkText(text);
            if (result.IsFailed)
                return result;
            
            var task = _client.GetGuild((ulong) sender.GroupId)
                .GetTextChannel((ulong) sender.UserSenderId)
                .SendFileAsync(mediaFile.Path, text);
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
        
        public Result<string> SendOnlineMedia(IBotOnlineFile file, string text, SenderInfo sender)
        {
            if (text.Length != 0)
            {
                var result = sendText(text, sender);
                if (result.IsFailed) return result;
            }
            
            return sendText(file.Path, sender);
        }
        public Result<string> SendTextMessage(string text, SenderInfo sender)
        {
            if (text.Length == 0)
            {
                LoggerHolder.Instance.Error($"The message wasn't sent by the command " +
                                            $"\"{PingCommand.Descriptor.CommandName}\", the length must not be zero.");
                return Result.Ok();
            }
            
            return sendText(text, sender);
        }
        
        private Result<string> sendText(string text, SenderInfo sender)
        {
            Result<string> result = checkText(text);
            if (result.IsFailed)
                return result;
            
            var task = _client.GetGuild((ulong) sender.GroupId)
                .GetTextChannel((ulong) sender.UserSenderId)
                .SendMessageAsync(text);

            try
            {
                task.Wait();
                return Result.Ok("Message send");
            }
            catch (Exception e)
            {
                string message = "Error while sending message";
                LoggerHolder.Instance.Error(e, message);
                return Result.Fail(new Error(message).CausedBy(e));
            }
        }

        private Result<string> checkText(string text)
        {
            if (text.Length > 2000)
            {
                string subString = text.Substring(0, 99) + "...";
                string errorMessage = $"The message wasn't sent by the command " +
                                      $"\"{PingCommand.Descriptor.CommandName}\", the length is too big.";
                return Result.Fail(new Error(errorMessage).CausedBy(subString));
            }

            return Result.Ok();
        }
    }
}
