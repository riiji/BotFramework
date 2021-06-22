[![Nuget](https://img.shields.io/nuget/v/Kysect.BotFramework?style=flat-square)](https://www.nuget.org/packages/Kysect.BotFramework)

# BotFramework

BotFramework - фреймворк, который упрощает процедуру создания ботов за счет абстрагирования над API конкретных месенджеров.

## Startup

Начало работы:

### Настройка авторизации

TSettings - класс, в котором описаны настройки бота. Пример такого класса (данный класс доступен из коробки)

```csharp
public class TelegramSettings
    {
        public TelegramSettings(string accessToken)
        {
            AccessToken = accessToken;
        }

        public TelegramSettings()
        {
        }

        public string AccessToken { get; set; }
    }
```

### Получение настроек через конфигурационный файл

Задать настройки можно с помощью json файла.

```csharp
var settings = new ConfigSettingsProvider<TSettings>("filename");
```

Вид JSON конфига для TelegramSettings
```json
{
    "AcessToken" : "token"
}
```

### Получение настроек из строки

Для упрощения процесса разработки, настройки можно получить из строки с токеном

```csharp
var settings = new ConstSettingsProvider<TSettings>(new TSettings(token));
```

### Добавление новых команд:

Чтобы добавить команду, ваш класс должен реализовывать интерфейс IBotSyncCommand или IBotAsyncCommand

Пример команды Ping
```csharp
public class PingCommand : IBotAsyncCommand
    {
        public static readonly BotCommandDescriptor<PingCommand> Descriptor = new BotCommandDescriptor<PingCommand>(
            "Ping",
            "Answer pong on ping message");

        public Result CanExecute(CommandArgumentContainer args)
        {
            return Result.Ok();
        }

        public Task<Result<IBotMessage>> Execute(CommandArgumentContainer args)
        {
            IBotMessage message = new BotTextMessage("Pong!");
            return Task.FromResult(Result.Ok(message));
        }
    }
```

Пример создания бота с командой Ping

```csharp
var telegramToken = string.Empty;

var settings = new ConstSettingsProvider<TelegramSettings>(new TelegramSettings(telegramToken));
var api = new TelegramApiProvider(settings);

BotManager botManager = new BotManagerBuilder()
    .SetPrefix('!')
    .SetCaseSensitive(false)
    .AddCommand(PingCommand.Descriptor)
    .Build(api);

botManager.Start();
```
