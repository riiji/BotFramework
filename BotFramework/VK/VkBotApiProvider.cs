using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using Tef.BotFramework.Abstractions;
using Tef.BotFramework.Common;
using Tef.BotFramework.Core;
using Tef.BotFramework.Settings;
using Tef.BotFramework.Tools.Extensions;
using Tef.BotFramework.Tools.Loggers;
using VkApi.Wrapper;
using VkApi.Wrapper.Auth;
using VkApi.Wrapper.LongPolling.Bot;
using VkApi.Wrapper.Objects;

namespace Tef.BotFramework.VK
{
    public class VkBotApiProvider : IBotApiProvider, IDisposable
    {
        public event EventHandler<BotEventArgs> OnMessage;

        private Vkontakte _vkApi;
        private BotLongPollClient _client;
        private readonly VkSettings _settings;
        private readonly Logger _vkFileLogger;

        public VkBotApiProvider(IGetSettings<VkSettings> settings)
        {
            _settings = settings.GetSettings();
            _vkFileLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File("vk-api.txt")
                .CreateLogger();

            Initialize();
        }
        
        public Result WriteMessage(SenderData sender, string message)
        {
            var sendMessageTask = _vkApi.Messages.Send(
                randomId: Utilities.GetRandom(),
                peerId: sender.GroupId,
                message: message);

            sendMessageTask.WaitSafe();

            if (sendMessageTask.IsFaulted)
                return new Result(false, $"Vk write message failed from {sender.GroupId} with message {message}").WithException(sendMessageTask.Exception);
            return new Result(true, $"Vk write {message} to {sender.GroupId} ok");
        }

        private Result Initialize()
        {
            var accessToken = AccessToken.FromString(_settings.VkKey);
            _vkApi = new Vkontakte(_settings.VkAppId, new VkLibraryLogger(_vkFileLogger), _settings.VkAppSecret) { AccessToken = accessToken };
            Task<GroupsLongPollServer> getSettingsTask = _vkApi.Groups.GetLongPollServer(_settings.VkGroupId);

            getSettingsTask.WaitSafe();

            if (getSettingsTask.IsFaulted)
                return new Result(false, "Get long poll server failed").WithException(getSettingsTask.Exception);

            var settings = getSettingsTask.Result;

            Task<BotLongPollClient> clientTask = _vkApi.StartBotLongPollClient
            (
                settings.Server,
                settings.Key,
                Convert.ToInt32(settings.Ts)
            );

            clientTask.WaitSafe();

            if (clientTask.IsFaulted)
            {
                return new Result(false, "Auth is failed").WithException(clientTask.Exception);
            }

            _client = clientTask.Result;
            _client.OnMessageNew += Client_OnMessageNew;
            _client.LongPollFailureReceived += Client_OnFail;
            _client.ResponseReceived += Client_OnResponse;

            return new Result(true, "Auth successfully");
        }

        private void Client_OnMessageNew(object sender, MessagesMessage e)
        {
            _vkFileLogger.Debug("New message event: {@e}", e);
            OnMessage?.Invoke(sender, new BotEventArgs(e.Text, e.PeerId, e.FromId));
        }

        private void Client_OnResponse(object sender, JArray e)
        {
            _vkFileLogger.Debug("New response event: {@e}", e);
            LoggerHolder.Log.Information($"Response: {string.Join(' ',e.ToArray().Select(x=>x.ToString()))}");
        }

        public void OnFail()
        {
            Client_OnFail(null, 2);
        }
        
        private void Client_OnFail(object sender, int e)
        {
            LoggerHolder.Log.Error($"VkBotApiProvider_Client_OnFail with {e}");

            var settings = _vkApi.Groups.GetLongPollServer(_settings.VkGroupId).Result;
            var client = _vkApi.StartBotLongPollClient
            (
                settings.Server,
                settings.Key,
                Convert.ToInt32(settings.Ts)
            ).Result;

            _client = client;
            _client.OnMessageNew += Client_OnMessageNew;
            _client.LongPollFailureReceived += Client_OnFail;
            _client.ResponseReceived += Client_OnResponse;
        }

        public void Dispose()
        {
            _client.OnMessageNew -= Client_OnMessageNew;
            _client.LongPollFailureReceived -= Client_OnFail;
            _client.ResponseReceived -= Client_OnResponse;
            _vkApi.Dispose();
        }
    }
}