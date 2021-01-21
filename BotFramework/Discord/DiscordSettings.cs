namespace Tef.BotFramework.Discord
{
    public class DiscordSettings
    {
        public DiscordSettings(string accessToken)
        {
            AccessToken = accessToken;
        }

        public DiscordSettings()
        {
        }

        public string AccessToken { get; set; }
    }
}