using System.Threading.Tasks;
using Tef.BotFramework.ApiProviders.Telegram;
using Tef.BotFramework.Core;
using Tef.BotFramework.Core.BotCommands;
using Tef.BotFramework.Settings;

namespace BotFrameworkConsoleTest
{
    public static class Program
    {
        private static async Task MainAsync()
        {
            var telegramToken = string.Empty;

            var settings = new ConstSettingsProvider<TelegramSettings>(new TelegramSettings(telegramToken));
            var api = new TelegramApiProvider(settings);

            new Bot(api)
                .AddDefaultLogger()
                .SetPrefix('!')
                .WithoutCaseSensitiveCommands()
                .AddCommand(new PingCommand())
                .Start();

            await Task.Delay(-1);
        }

        private static void Main() => MainAsync().GetAwaiter().GetResult();
    }
}