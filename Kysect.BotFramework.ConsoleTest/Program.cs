using System.Threading.Tasks;
using Kysect.BotFramework.ApiProviders.Telegram;
using Kysect.BotFramework.Commands;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.ConsoleTest
{
    public static class Program
    {
        private static async Task MainAsync()
        {
            var telegramToken = "1484135943:AAGM6JHSj-ER8ekwHQbaJPxHDxfCEz1hSHM";

            var settings = new ConstSettingsProvider<TelegramSettings>(new TelegramSettings(telegramToken));
            var api = new TelegramApiProvider(settings);
            var serviceCollection = new ServiceCollection();

            new BotManager(api, serviceCollection.BuildServiceProvider())
                .AddDefaultLogger()
                .SetPrefix('!')
                .WithoutCaseSensitiveCommands()
                .AddCommand(PingCommand.Descriptor)
                .Start();

            await Task.Delay(-1);
        }

        private static void Main() => MainAsync().GetAwaiter().GetResult();
    }
}