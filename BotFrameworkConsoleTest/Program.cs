using System.Threading.Tasks;
using Tef.BotFramework.BotCommands;
using Tef.BotFramework.Core;
using Tef.BotFramework.Settings;
using Tef.BotFramework.VK;

namespace BotFrameworkConsoleTest
{
    class Program
    {
        private static async Task MainAsync()
        {
            var vkSettings = new SettingsFromConfig<VkSettings>("TelegramConfig.json");
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