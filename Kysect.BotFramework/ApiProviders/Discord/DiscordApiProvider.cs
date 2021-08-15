using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DSharpPlus;
using DSharpPlus.Entities;
using FluentResults;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.DefaultCommands;
using Kysect.BotFramework.Settings;
using TokenType = Discord.TokenType;
using System.Text.RegularExpressions;

namespace Kysect.BotFramework.ApiProviders.Discord
{
    public class DiscordApiProvider : IBotApiProvider, IDisposable
    {
        private readonly object _lock = new object();
        private readonly DiscordSettings _settings;
        private DiscordSocketClient _client;
        private int _argsCount;
        
        private readonly string[] _emojis =
        {
            "", "1️⃣", "2️⃣", "3️⃣","4️⃣","5️⃣","6️⃣","7️⃣", "8️⃣", "9️⃣", "🔟"
        };

        public DiscordApiProvider(ISettingsProvider<DiscordSettings> settingsProvider)
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
            
            foreach (IBotMediaFile media in mediaFiles.Skip(1))
            {
                if (result.IsFailed)
                {
                    return result;
                }

                if (media is IBotOnlineFile onlineMediaFile)
                {
                    result = SendOnlineMedia(onlineMediaFile, string.Empty, sender);
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
            Result<string> result = CheckText(text);
            if (result.IsFailed)
            {
                return result;
            }

            Task<RestUserMessage> task = _client.GetGuild((ulong) sender.GroupId)
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
                Result<string> result = SendText(text, sender);
                if (result.IsFailed)
                {
                    return result;
                }
            }

            return SendText(file.Path, sender);
        }

        public Result<string> SendTextMessage(string text, SenderInfo sender)
        {
            if (text.Length == 0)
            {
                LoggerHolder.Instance.Error("The message wasn't sent by the command " +
                                            "\"{CommandName}\", the length must not be zero", 
                                            PingCommand.Descriptor.CommandName);
                return Result.Ok();
            }
            
            return SendText(text, sender);
        }
        
        public Result<string> SendPollMessage(string text, SenderInfo sender)
        {
            if (text.Length == 0)
            {
                LoggerHolder.Instance.Error("The message wasn't sent by the command " +
                                            "\"{CommandName}\", the length must not be zero", 
                                            PollCommand.Descriptor.CommandName);
                return Result.Ok();
            }
            
            return SendPoll(text, sender);
        }

        public void Dispose()
        {
            _client.MessageReceived -= ClientOnMessage;
            _client.StopAsync();
        }

        private void Initialize()
        {
            _client = new DiscordSocketClient();

            _client.LoginAsync(TokenType.Bot, _settings.AccessToken);

            _client.MessageReceived += ClientOnMessage;
            _client.StartAsync();
        }

        private Task ClientOnMessage(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message is null)
            {
                return Task.CompletedTask;
            }

            var context = new SocketCommandContext(_client, message);
            if (!context.User.IsBot && context.Message.Content.Contains("!Poll"))
            {
                _argsCount = GetPollArguments(message.Content).Count() - 1;
            }
            if (context.User.IsBot && context.Message.Embeds.Any())
            {
                ReactWithEmojis(context);
            }
            
            if (context.User.IsBot || context.Guild is null)
            {
                return Task.CompletedTask;
            }
            LoggerHolder.Instance.Debug($"New message event: {context.Message}");
            
            IBotMessage botMessage = ParseMessage(message, context);
            OnMessage?.Invoke(context.Client,
                              new BotEventArgs(
                                  botMessage,
                                  new SenderInfo(
                                      (long) (context.Guild.Id),
                                      (long) context.Channel.Id,
                                      context.User.Username,
                                      CheckIsAdmin(context.User)
                                  )
                              ));

            return Task.CompletedTask;
        }

        private IBotMessage ParseMessage(SocketUserMessage message, SocketCommandContext context)
        {
            if (context.Message.Attachments.Count == 0)
            {
                return new BotTextMessage(context.Message.Content);
            }

            if (context.Message.Attachments.Count == 1)
            {
                IBotOnlineFile onlineFile =
                    GetOnlineFile(message.Attachments.First().Filename, message.Attachments.First().Url);
                return onlineFile is not null
                    ? new BotSingleMediaMessage(context.Message.Content, onlineFile)
                    : new BotTextMessage(context.Message.Content);
            }

            List<IBotMediaFile> mediaFiles = context.Message.Attachments
                                                    .Select(attachment =>
                                                                GetOnlineFile(attachment.Filename, attachment.Url))
                                                    .Where(onlineFile => onlineFile is not null).Cast<IBotMediaFile>()
                                                    .ToList();

            if (!mediaFiles.Any())
            {
                return new BotTextMessage(context.Message.Content);
            }


            if (mediaFiles.Count == 1)
            {
                return new BotSingleMediaMessage(context.Message.Content, mediaFiles.First());
            }


            return new BotMultipleMediaMessage(context.Message.Content, mediaFiles);
        }

        private IBotOnlineFile GetOnlineFile(string filename, string url)
        {
            switch (ParseMediaType(filename))
            {
                case MediaTypeEnum.Photo: return new BotOnlinePhotoFile(url);
                case MediaTypeEnum.Video: return new BotOnlineVideoFile(url);
                case MediaTypeEnum.Undefined:
                default:
                    LoggerHolder.Instance.Information($"Skipped file: {filename}");
                    return null;
            }
        }

        private MediaTypeEnum ParseMediaType(string filename)
        {
            if (filename.EndsWith("png") || filename.EndsWith("jpg") ||
                filename.EndsWith("bmp"))
            {
                return MediaTypeEnum.Photo;
            }

            if (filename.EndsWith("mp4") || filename.EndsWith("mov") ||
                filename.EndsWith("wmv") || filename.EndsWith("avi"))
            {
                return MediaTypeEnum.Video;
            }

            return MediaTypeEnum.Undefined;
        }

        private bool CheckIsAdmin(SocketUser user)
        {
            var socketGuildUser = user as SocketGuildUser;
            return socketGuildUser.GuildPermissions.Administrator;
        }

        private Result<string> SendText(string text, SenderInfo sender)
        {
            Result<string> result = CheckText(text);
            if (result.IsFailed)
            {
                return result;
            }

            Task<RestUserMessage> task = _client.GetGuild((ulong) sender.GroupId)
                                                .GetTextChannel((ulong) sender.UserSenderId)
                                                .SendMessageAsync(text);

            try
            {
                task.Wait();
                return Result.Ok("Message send");
            }
            catch (Exception e)
            {
                var message = "Error while sending message";
                LoggerHolder.Instance.Error(e, message);
                return Result.Fail(new Error(message).CausedBy(e));
            }
        }
        
        private Result<string> SendPoll(string text, SenderInfo sender)
        {
            List<string> arguments = GetPollArguments(text);

            Result<string> result = CheckText(text);
            if (result.IsFailed)
            {
                return result;
            }

            for (int i = 1; i < arguments.Count; i++)
            {
                arguments[i] = _emojis[i] + "\t" + arguments[i];
            }
            
            var embed = new EmbedBuilder
            {
                Title = arguments[0],
                Color = Color.Purple,
                Description = String.Join("\n", arguments.Skip(1))
            };

            Task<RestUserMessage> task = _client.GetGuild((ulong) sender.GroupId)
                .GetTextChannel((ulong) sender.UserSenderId)
                .SendMessageAsync(embed: embed.Build());

            try
            {
                task.Wait();
                return Result.Ok("Message send");
            }
            catch (Exception e)
            {
                var message = "Error while sending message";
                LoggerHolder.Instance.Error(e, message);
                return Result.Fail(new Error(message).CausedBy(e));
            }
        }

        private Result<string> CheckText(string text)
        {
            if (text.Length > 2000)
            {
                string subString = text.Substring(0, 99) + "...";
                string errorMessage = "The message wasn't sent by the command " +
                                      $"\"{PingCommand.Descriptor.CommandName}\", the length is too big";
                return Result.Fail(new Error(errorMessage).CausedBy(subString));
            }

            return Result.Ok();
        }
        
        private List<string> GetPollArguments(string args)
        {
            var regex = new Regex(@"[^\s""']+|""([^""]*)""|'([^']*)'"); // Splits into "..." '...' a b c
            var matches = regex.Matches(args);
            List<string> options = new List<string>();
                
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    options.Add(match.Value.Replace("\"", ""));
                }
            }

            return options;
        }
        
        private async void ReactWithEmojis(SocketCommandContext context)
        {
            for (int i = 1; i < _argsCount; i++)
            {
                await context.Message.AddReactionAsync(new Emoji(_emojis[i]))
                    .ConfigureAwait(false);
            }
        }
    }
}