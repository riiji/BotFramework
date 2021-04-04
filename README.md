# BotFramework

BotFramework - фреймворк, который упрощает процедуру создания ботов за счет абстрагирования над API конкретных месенджеров.

## Startup

Начало работы:

### Получение настроек через конфигурационный файл

Задать настройки можно с помощью json файла.

```csharp
var settings = new ConfigSettingsProvider<TSettings>("filename");
```

TSettings - класс, в котором описаны настройки бота. Пример такого класса (данный класс доступен из коробки)

```csharp
public class VkSettings
{
        public VkSettings(string vkKey, int vkAppId, string vkAppSecret, int vkGroupId)
        {
            VkKey = vkKey;
            VkAppId = vkAppId;
            VkAppSecret = vkAppSecret;
            VkGroupId = vkGroupId;
        }

        public VkSettings()
        {
        }

        public string VkKey { get; set; }
        public int VkAppId { get; set; }
        public string VkAppSecret { get; set; }
        public int VkGroupId { get; set; }
}
```

```csharp
var settings = new SettingsFromConfig<VkSettings>("filename");
```

Вид JSON конфига для VkSettings
```json
{
"VkKey":"",
"VkAppId":"",
"VkAppSecret":"",
"VkGroupId":""
}
```

### Добавление новых команд:

Чтобы добавить команду, ваш класс должен реализовывать интерфейс IBotCommand

Пример команды Ping
```csharp
public class PingCommand : IBotCommand
{
        public string CommandName { get; } = "Ping";
        public string Description { get; } = "Answer pong on ping message";
        public string[] Args { get; } = new string[0];

        public Result CanExecute(CommandArgumentContainer args)
        {
            return Result.Ok();
        }

        public Task<Result<string>> ExecuteAsync(CommandArgumentContainer args)
        {
            return Task.FromResult(Result.Ok($"Pong {args.Sender.Username}"));
        }
}
```

Пример создания бота с командой Ping

```csharp
var telegramToken = string.Empty;

var settings = new ConstSettingsProvider<TelegramSettings>(new TelegramSettings(telegramToken));
var api = new TelegramApiProvider(settings);

new BotManager(api)
    .AddDefaultLogger()
    .SetPrefix('!')
    .WithoutCaseSensitiveCommands()
    .AddCommand(new PingCommand())
    .Start();
```
