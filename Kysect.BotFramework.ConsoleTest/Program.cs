using System.Threading.Tasks;
using Kysect.BotFramework.ApiProviders.Telegram;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotCommands;
using Kysect.BotFramework.Settings;

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