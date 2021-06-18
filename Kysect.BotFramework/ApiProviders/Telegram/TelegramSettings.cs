namespace Kysect.BotFramework.ApiProviders.Telegram
{
    public class TelegramSettings
    {
        public string AccessToken { get; set; }

        public TelegramSettings(string accessToken)
        {
            AccessToken = accessToken;
        }

        public TelegramSettings()
        {
        }
    }
}