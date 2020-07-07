BotFramework

Простой фреймворк для ~~вк~~ любого бота

Начало работы:

1. Получение настроек:

* [Через конфиг](https://github.com/TEF-Dev/BotFramework/blob/master/SettingsFromConfig.md)

* Через Sql, Postgres, Sqlite

* Через пользовательский метод

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
            var config = new SettingsFromConfig<VkSettings>("config.json");
            var settings = config.GetSettings();
            
            // создание провайдера вк для бота
            var api = new VkBotApiProvider(settings);

            // создание списка команд
            var commands = new CommandsList();
            
            // добавление команды
            commands.AddCommand(new PingCommand());

            // запуск бота с логированием и установленным префиксом (по умолчанию !)
            new Bot(api, new CommandParser(), commands)
                .AddLogger()
                .SetPrefix('.')
                .Start();
                
            await Task.Delay(-1);
        }

        private static void Main() => MainAsync().GetAwaiter().GetResult();
}
```
