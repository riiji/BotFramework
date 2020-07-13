using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Tef.BotFramework.BotCommands;
using Tef.BotFramework.Core;
using Tef.BotFramework.Core.CommandControllers;
using Tef.BotFramework.Database;
using Tef.BotFramework.Settings;
using Tef.BotFramework.Tools.Loggers;
using Tef.BotFramework.VK;

namespace BotFrameworkConsoleTest
{
    class Program
    {
        static async Task MainAsync()
        {
            var settings = new SettingsFromSqlite<VkSettings>("MyDb.db");
            
            // создание провайдера вк для бота
            var api = new VkBotApiProvider(settings);

            // запуск бота с логированием и установленным префиксом
            new Bot(api)
                .AddLogger()
                .SetPrefix('.')
                .AddCommand(new PingCommand())
                .Start();
                
            await Task.Delay(-1);
        }

        private static void Main() => MainAsync().GetAwaiter().GetResult();
    }
}