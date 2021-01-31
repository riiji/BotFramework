using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using FluentResults;
using Tef.BotFramework.Core;
using Tef.BotFramework.Core.Abstractions;
using Tef.BotFramework.Settings;
using Tef.BotFramework.Tools.Loggers;

namespace Tef.BotFramework.ApiProviders.Discord
{
    public class DiscordApiProvider : IBotApiProvider, IDisposable
    {
        private readonly object _lock = new object();
        private DiscordSocketClient _client;
        private readonly DiscordSettings _settings;

        public event EventHandler<BotEventArgs> OnMessage;

        public DiscordApiProvider(IGetSettings<DiscordSettings> settings)
        {
            _settings = settings.GetSettings();
            _client = new DiscordSocketClient();

            _client.LoginAsync(TokenType.Bot, _settings.AccessToken);

            _client.MessageReceived += ClientOnMessage;
            _client.StartAsync();
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
                    context.Message.ToString(),
                    (long) (context.Guild?.Id ?? 0),
                    (long) context.Channel.Id,
                    context.User.Username
                ));
            return Task.CompletedTask;
        }

        public Result<string> WriteMessage(BotEventArgs sender)
        {
            Task<RestUserMessage> task = _client.GetGuild((ulong) sender.GroupId)
                .GetTextChannel((ulong) sender.UserSenderId)
                .SendMessageAsync(sender.Text);
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

        public void Restart()
        {
            lock (_lock)
            {
                if (_client != null)
                    Dispose();

                _client = new DiscordSocketClient();

                _client.LoginAsync(TokenType.Bot, _settings.AccessToken);

                _client.MessageReceived += ClientOnMessage;
                _client.StartAsync();
            }
        }

        public void Dispose()
        {
            _client.MessageReceived -= ClientOnMessage;
            _client.StopAsync();
        }
    }
}
