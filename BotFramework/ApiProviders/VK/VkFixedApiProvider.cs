using System;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Tef.BotFramework.Core;
using Tef.BotFramework.Core.Abstractions;
using Tef.BotFramework.Settings;
using Tef.BotFramework.Tools.Extensions;
using VkApi.Wrapper;
using VkApi.Wrapper.LongPolling.Bot;
using VkApi.Wrapper.Objects;

namespace Tef.BotFramework.ApiProviders.VK
{
    public class VkFixedApiProvider : IBotApiProvider, IDisposable
    {
        public event EventHandler<BotEventArgs> OnMessage;

        private readonly object _lock = new object();
        private readonly VkSettings _settings;
        private Vkontakte _api;
        private BotLongPollClient _client;

        public VkFixedApiProvider(ISettingsProvider<VkSettings> settingsProvider)
        {
            _settings = settingsProvider.GetSettings();
            Initialize();
        }

        private void Initialize()
        {
            //TODO: log inner error?
            //TODO: Add inner error message to ArgumentException?
            //TODO: Replace ArgumentException with custom exception (BorFrameworkException?)
            _api = new Vkontakte(_settings.VkAppId, _settings.VkAppSecret);
            Task<GroupsLongPollServer> serverTask = _api.Groups.GetLongPollServer();
            if (!serverTask.IsCompletedSuccessfully)
                throw new ArgumentException("internal error");
            GroupsLongPollServer server = serverTask.Result;
            Task<BotLongPollClient> clientTask = _api.StartBotLongPollClient(server.Server, server.Key, int.Parse(server.Ts));
            if (!clientTask.IsCompletedSuccessfully)
                throw new ArgumentException("internal error");
            BotLongPollClient client = clientTask.Result;
            client.OnMessageNew += Client_OnMessageNew;
            client.LongPollFailureReceived += Client_LongPollFailureReceived;
        }

        public void Restart()
        {
            //TODO: looks like this method must be `public async Task RestartSafe`
            //fyi: *Safe means that method never throw exception. Probably, we need to use await + try/catch
            lock (_lock)
            {
                if (_client != null)
                {
                    Dispose();
                }

                Initialize();
            }
        }

        private void Client_LongPollFailureReceived(object sender, int e)
        {
            Restart();
        }

        private void Client_OnMessageNew(object sender, VkApi.Wrapper.Objects.MessagesMessage e)
        {
            //TODO: add some logs with Debug level?
            Task<UsersUserXtrCounters[]> userTask = _api.Users.Get(new[] { e.FromId.ToString() });
            userTask.WaitSafe();

            //TODO: log error?
            if (!userTask.IsCompletedSuccessfully)
                return;
            
            UsersUserXtrCounters[] users = userTask.Result;
            //TODO: single? Ensure is not null?
            UsersUserXtrCounters user = users.FirstOrDefault();

            OnMessage?.Invoke(sender, new BotEventArgs(e.Text, e.PeerId, e.FromId, user?.FirstName));
        }

        public Result<string> WriteMessage(BotEventArgs args)
        {
            Task<int> sendMessageTask = _api.Messages.Send
            (
                randomId: Utilities.GetRandom(),
                peerId: (int)args.GroupId,
                message: args.Text
            );

            sendMessageTask.WaitSafe();

            return sendMessageTask.IsFaulted
                ? Result.Fail<string>(new Error($"Vk write message failed from {args.GroupId} with message {args.Text}").CausedBy(sendMessageTask.Exception))
                : Result.Ok($"Vk write {args.Text} to {args.GroupId} ok");
        }

        public void Dispose()
        {
            //TODO: add flag _isDisposed
            _client.OnMessageNew -= Client_OnMessageNew;
            _client.LongPollFailureReceived -= Client_LongPollFailureReceived;
            _client.Stop();
            _api.Dispose();
        }
    }
}