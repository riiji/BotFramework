using System;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Extensions;
using Kysect.BotFramework.Settings;
using VkApi.Wrapper;
using VkApi.Wrapper.LongPolling.Bot;
using VkApi.Wrapper.Objects;

namespace Kysect.BotFramework.ApiProviders.VK
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
            serverTask.WaitSafe();
            if (serverTask.IsFaulted)
                throw new ArgumentException("internal error");
            
            GroupsLongPollServer server = serverTask.Result;
            Task<BotLongPollClient> clientTask = _api.StartBotLongPollClient(server.Server, server.Key, int.Parse(server.Ts));
            clientTask.WaitSafe();
            if (clientTask.IsFaulted)
                throw new ArgumentException("internal error");

            _client = clientTask.Result;
            _client.OnMessageNew += Client_OnMessageNew;
            _client.LongPollFailureReceived += Client_LongPollFailureReceived;
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

        private void Client_OnMessageNew(object sender, MessagesMessage e)
        {
            //TODO: add some logs with Debug level?
            Task<UsersUserXtrCounters[]> userTask = _api.Users.Get(new[] { e.FromId.ToString() });
            userTask.WaitSafe();

            //TODO: log error?
            if (userTask.IsFaulted)
                return;
            
            UsersUserXtrCounters[] users = userTask.Result;
            //TODO: single? Ensure is not null?
            UsersUserXtrCounters user = users.FirstOrDefault();

            OnMessage?.Invoke(sender, new BotEventArgs(new BotTextMessage(e.Text), new SenderInfo(e.PeerId, e.FromId, user?.FirstName)));
        }

        public Result<string> SendText(String text, SenderInfo sender)
        {
            Task<int> sendMessageTask = _api.Messages.Send
            (
                randomId: RandomUtilities.GetRandom(),
                peerId: (int)sender.GroupId,
                message: text
            );

            sendMessageTask.WaitSafe();

            return sendMessageTask.IsFaulted
                ? Result.Fail<string>(new Error($"Vk write message failed from {sender.GroupId} with message {text}").CausedBy(sendMessageTask.Exception))
                : Result.Ok($"Vk write {text} to {sender.GroupId} ok");
        }

        public Result<string> SendImage(string imagePath, string text, SenderInfo sender)
        {
            throw new NotImplementedException();
        }

        public Result<string> SendVideo(string videoPath, string text, SenderInfo sender)
        {
            throw new NotImplementedException();
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