using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FluentResults;
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

        public DiscordApiProvider(ISettingsProvider<DiscordSettings> settingsProvider)
        {
            _settings = settingsProvider.GetSettings();
            Initialize();
        }

        public event EventHandler<BotEventArgs> OnMessage;

        public Result<string> SendText(string text, SenderInfo sender)
        {
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
                const string message = "Error while sending message";
                LoggerHolder.Instance.Error(e, message);
                return Result.Fail(new Error(message).CausedBy(e));
            }
        }

        public Result<string> SendMedia(IBotMediaFile mediaFile, string text, SenderInfo sender)
        {
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

        public Result<string> SendMultipleMedia(List<IBotMediaFile> mediaFiles, string text, SenderInfo sender)
        {
            Result<string> result;
            if (mediaFiles.First() is IBotOnlineFile)
            {
                result = SendText(text, sender);
                if (result.IsFailed)
                {
                    return result;
                }
                result = SendText(mediaFiles.First().Path, sender);
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
                    result = SendText(media.Path, sender);
                }
                else
                {
                    result = SendMedia(media, string.Empty, sender);
                }
            }

            return result;
        }

        public Result<string> SendOnlineMedia(IBotOnlineFile file, string text, SenderInfo sender)
        {
            var result = SendText(text, sender);
            if (result.IsFailed) return result;
            result = SendText(file.Path, sender);
            return result;
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
            //TODO: Refactor
            IBotMessage botMessage = new BotTextMessage(String.Empty);
            
            if (context.Message.Attachments.Count == 0)
            {
                botMessage = new BotTextMessage(context.Message.Content);
            }
            else
            {
                if (context.Message.Attachments.Count > 1)
                {
                    List<IBotMediaFile> mediaFiles = new List<IBotMediaFile>();
                    foreach (var attachment in context.Message.Attachments)
                    {
                        if (attachment.Filename.EndsWith("png") || attachment.Filename.EndsWith("jpg") ||
                            attachment.Filename.EndsWith("bmp"))
                        {
                            mediaFiles.Add(new BotOnlinePhotoFile(attachment.Url));
                        }
                    }

                    if (mediaFiles.Count == 0)
                    {
                        botMessage = new BotTextMessage(context.Message.Content);
                    } else if (mediaFiles.Count == 1)
                    {
                        botMessage = new BotSingleMediaMessage(context.Message.Content, mediaFiles.First());
                    }
                    else
                    {
                        botMessage = new BotMultipleMediaMessage(context.Message.Content, mediaFiles);
                    }
                }
                else
                {
                    if (context.Message.Attachments.First().Filename.EndsWith("png") || context.Message.Attachments.First().Filename.EndsWith("jpg") ||
                        context.Message.Attachments.First().Filename.EndsWith("bmp"))
                    {
                        botMessage = new BotSingleMediaMessage(context.Message.Content,new BotOnlinePhotoFile(context.Message.Attachments.First().Url));
                    }
                }
            }
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

        private void Initialize()
        {
            _client = new DiscordSocketClient();

            _client.LoginAsync(TokenType.Bot, _settings.AccessToken);

            _client.MessageReceived += ClientOnMessage;
            _client.StartAsync();
        }
    }
}