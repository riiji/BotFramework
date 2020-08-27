using System;
using System.Linq;
using System.Threading.Tasks;
using Tef.BotFramework.Abstractions;
using Tef.BotFramework.Common;
using Tef.BotFramework.Core;
using Tef.BotFramework.Settings;
using Tef.BotFramework.Tools.Extensions;
using VkApi.Wrapper;
using VkApi.Wrapper.LongPolling.Bot;

namespace Tef.BotFramework.VK
{
    public class VkFixedApiProvider : IBotApiProvider, IDisposable
    {
        public event EventHandler<BotEventArgs> OnMessage;

        private readonly VkSettings _settings;
        private Vkontakte _api;
        private BotLongPollClient _client;

        public VkFixedApiProvider(IGetSettings<VkSettings> settings)
        {
            _settings = settings.GetSettings();
            _api = new Vkontakte(_settings.VkAppId, _settings.VkAppSecret);
            var serverTask = _api.Groups.GetLongPollServer();
            if (!serverTask.IsCompletedSuccessfully)
                throw new ArgumentException("internal error");
            var server = serverTask.Result;
            var clientTask = _api.StartBotLongPollClient(server.Server, server.Key, int.Parse(server.Ts));
            if (!clientTask.IsCompletedSuccessfully)
                throw new ArgumentException("internal error");
            var client = clientTask.Result;
            client.OnMessageNew += Client_OnMessageNew;
            client.LongPollFailureReceived += Client_LongPollFailureReceived;
        }

        public void Restart()
        {
            if (_client != null)
            {
                _client.OnMessageNew -= Client_OnMessageNew;
                _client.LongPollFailureReceived -= Client_LongPollFailureReceived;
            }

            _api = new Vkontakte(_settings.VkAppId, _settings.VkAppSecret);
            var serverTask = _api.Groups.GetLongPollServer();
            if (!serverTask.IsCompletedSuccessfully)
                throw new ArgumentException("internal error");
            var server = serverTask.Result;
            var clientTask = _api.StartBotLongPollClient(server.Server, server.Key, int.Parse(server.Ts));
            if (!clientTask.IsCompletedSuccessfully)
                throw new ArgumentException("internal error");
            _client = clientTask.Result;
            _client.OnMessageNew += Client_OnMessageNew;
            _client.LongPollFailureReceived += Client_LongPollFailureReceived;
        }

        private void Client_LongPollFailureReceived(object sender, int e)
        {
            Restart();
        }

        private void Client_OnMessageNew(object sender, VkApi.Wrapper.Objects.MessagesMessage e)
        {
            var userTask = _api.Users.Get(new[] { e.FromId.ToString() });
            userTask.WaitSafe();

            if (!userTask.IsCompletedSuccessfully)
                return;
            var users = userTask.Result;
            var user = users.FirstOrDefault();

            OnMessage?.Invoke(sender, new BotEventArgs(e.Text, e.PeerId, e.FromId, user?.FirstName));
        }

        public Result WriteMessage(BotEventArgs args)
        {
            Task<int> sendMessageTask = _api.Messages.Send
            (
                randomId: Utilities.GetRandom(),
                peerId: (int)args.GroupId,
                message: args.Text
            );

            sendMessageTask.WaitSafe();

            return sendMessageTask.IsFaulted
                ? Result.Fail($"Vk write message failed from {args.GroupId} with message {args.Text}", sendMessageTask.Exception)
                : Result.Ok($"Vk write {args.Text} to {args.GroupId} ok");
        }

        public void Dispose()
        {

        }
    }
}