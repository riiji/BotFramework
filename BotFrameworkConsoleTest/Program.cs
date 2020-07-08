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
            var settings = new SettingsFromSqlite<VkSettings>("filename");
            
            // создание провайдера вк для бота
            var api = new VkBotApiProvider(settings);

            // создание списка команд
            var commands = new CommandsList();
            
            // добавление команды
            commands.AddCommand(new PingCommand());

            // запуск бота с логированием и установленным префиксом (по умолчанию !)
            new Bot(api, commands)
                .AddLogger()
                .SetPrefix('.')
                .Start();
                
            await Task.Delay(-1);
        }

        private static void Main() => MainAsync().GetAwaiter().GetResult();
    }
}