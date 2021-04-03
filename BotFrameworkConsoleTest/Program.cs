using System.Threading.Tasks;
using Tef.BotFramework.ApiProviders.VK;
using Tef.BotFramework.Core;
using Tef.BotFramework.Core.BotCommands;
using Tef.BotFramework.Settings;

namespace BotFrameworkConsoleTest
{
    class Program
    {
        private static async Task MainAsync()
        {

            var vkSettings = new ConfigSettingsProvider<VkSettings>("TelegramConfig.json");
            var telegramApi = new VkFixedApiProvider(vkSettings);

            new Bot(telegramApi)
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