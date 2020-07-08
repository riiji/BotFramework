[nuget](https://www.nuget.org/packages/Tef.BotFramework/0.9.2)

BotFramework

Простой фреймворк для ~~вк~~ любого бота

Начало работы:

1. Получение настроек:

* [Через конфиг](https://github.com/TEF-Dev/BotFramework/blob/master/SettingsFromConfig.md)

* [Через Sqlite](https://github.com/TEF-Dev/BotFramework/blob/master/SettingsFromSqlite.md)

* [Через пользовательский метод](https://github.com/TEF-Dev/BotFramework/blob/master/SettingsFromUserMethod.md)

2. Добавление новых команд:

Чтобы добавить команду, ваш класс должен реализовывать интерфейс IBotCommand

Пример команды Ping
```csharp
public class PingCommand : IBotCommand
{
        public string CommandName { get; } = "Ping";
        public string Description { get; } = "Answer pong on ping message";
        public string[] Args { get; } = new string[0];

        public bool CanExecute(CommandArgumentContainer args)
        {
            return true;
        }

        public Result Execute(CommandArgumentContainer args)
        {
            return new Result(isSuccess: true, executeMessage: "Pong");
        }
}
```

Пример создания бота с командой Ping

```csharp
class Program
{
        static async Task MainAsync()
        {
            // получение настроек
            var settings = new SettingsFromSqlite<VkSettings>("filename");
            
            // создание провайдера
            var api = new VkBotApiProvider(settings);
            
            var bot = new Bot(api);

            // запуск бота с командой пинг, логером и префиксом
            bot.AddCommand(new PingCommand())
                .AddLogger()
                .SetPrefix('!')
                .Start();
                
            await Task.Delay(-1);
        }

        private static void Main() => MainAsync().GetAwaiter().GetResult();
}
```
