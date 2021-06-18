namespace Kysect.BotFramework.ApiProviders.Discord
{
    public class DiscordSettings
    {
        public string AccessToken { get; set; }

        public DiscordSettings(string accessToken)
        {
            AccessToken = accessToken;
        }

        public DiscordSettings()
        {
        }
    }
}