using System.Threading.Tasks;
using Kysect.BotFramework.ApiProviders.Telegram;
using Kysect.BotFramework.Commands;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Settings;

namespace Kysect.BotFramework.ConsoleTest
{
    public static class Program
    {
        private static async Task MainAsync()
        {
            var telegramToken = "1484135943:AAGM6JHSj-ER8ekwHQbaJPxHDxfCEz1hSHM";

            var settings = new ConstSettingsProvider<TelegramSettings>(new TelegramSettings(telegramToken));
            var api = new TelegramApiProvider(settings);

            new BotManager(api)
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