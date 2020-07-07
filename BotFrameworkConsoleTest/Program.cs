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
            var settings = new SettingsFromDatabase<VkSettings>("MyDb.db", DatabaseType.Sqlite);

            var api = new VkBotApiProvider(settings.GetSettings());

            var commands = new CommandsList();
            commands.AddCommand(new PingCommand());

            new Bot(api, new CommandParser(), commands)
                .AddLogger()
                .SetPrefix('.')
                .Start();
                
            await Task.Delay(-1);
        }

        private static void Main() => MainAsync().GetAwaiter().GetResult();
    }
}