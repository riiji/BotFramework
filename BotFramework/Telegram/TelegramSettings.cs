namespace Tef.BotFramework.Telegram
{
    public class TelegramSettings
    {
        public TelegramSettings(string accessToken)
        {
            AccessToken = accessToken;
        }

        public TelegramSettings()
        {
            
        }

        public string AccessToken { get; set; }
    }
}