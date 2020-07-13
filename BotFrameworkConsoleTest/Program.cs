using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Tef.BotFramework.BotCommands;
using Tef.BotFramework.Core;
using Tef.BotFramework.Core.CommandControllers;
using Tef.BotFramework.Database;
using Tef.BotFramework.Settings;
using Tef.BotFramework.Telegram;
using Tef.BotFramework.Tools.Loggers;
using Tef.BotFramework.VK;

namespace BotFrameworkConsoleTest
{
    class Program
    {
        static async Task MainAsync()
        {
            var telegramSettings = new SettingsFromConfig<TelegramSettings>("TelegramConfig.json");
            
            var telegramApi = new TelegramApiProvider(telegramSettings);

            new Bot(telegramApi)
                .AddLogger()
                .SetPrefix('.')
                .WithoutCaseSensitiveCommands()
                .AddCommand(new PingCommand())
                .Start();
                
            await Task.Delay(-1);
        }

        private static void Main() => MainAsync().GetAwaiter().GetResult();
    }
}