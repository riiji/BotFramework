using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using FluentResults;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Settings;
using Telegram.Bot.Types;

namespace Kysect.BotFramework.ApiProviders.Discord
{
    public class DiscordApiProvider : IBotApiProvider, IDisposable
    {
        private readonly object _lock = new object();
        private DiscordSocketClient _client;
        private readonly DiscordSettings _settings;

        public event EventHandler<BotEventArgs> OnMessage;

        public DiscordApiProvider(ISettingsProvider<DiscordSettings> settingsProvider)
        {
            _settings = settingsProvider.GetSettings();
            Initialize();
        }

        private Task ClientOnMessage(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message is null) return Task.CompletedTask;
            var context = new SocketCommandContext(_client, message);
            if (context.User.IsBot || context.Guild is null) return Task.CompletedTask;
            //TODO: add message logging
            OnMessage?.Invoke(context.Client,
                new BotEventArgs(
                    new BotTextMessage(context.Message.ToString()),
                    new SenderInfo(
                    (long) (context.Guild?.Id ?? 0),
                    (long) context.Channel.Id,
                    context.User.Username
                    )
                ));
            return Task.CompletedTask;
        }

        public Result<string> SendText(string text, SenderInfo sender)
        {
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
                const string message = "Error while sending message";
                LoggerHolder.Instance.Error(e, message);
                return Result.Fail(new Error(message).CausedBy(e));
            }
        }

        public Result<string> SendFile(string filePath,string text, SenderInfo sender)
        {
            Task<RestUserMessage> task = _client.GetGuild((ulong) sender.GroupId)
                .GetTextChannel((ulong) sender.UserSenderId)
                .SendFileAsync(filePath,text);
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
            return SendFile(imagePath, text, sender);
        }
        
        public Result<string> SendVideo(string videoPath, string text, SenderInfo sender)
        {
            return SendFile(videoPath, text, sender);
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

        private void Initialize()
        {
            _client = new DiscordSocketClient();

            _client.LoginAsync(TokenType.Bot, _settings.AccessToken);

            _client.MessageReceived += ClientOnMessage;
            _client.StartAsync();
        }

        public void Dispose()
        {
            _client.MessageReceived -= ClientOnMessage;
            _client.StopAsync();
        }
    }
}
