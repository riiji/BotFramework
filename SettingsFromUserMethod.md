Получение настроек с помощью пользовательского метода

Вполне может возникнуть ситуация, когда необходимо реализовать получение настроек из какого то другого ресурса, именно для этого используется данный способ

Для этого необходимо реализовать интерфейс IGetSettings<out TSettings>

TSettings - настройки, которые необходимо получить

Пример реализации интерфейса TSettings

```csharp
public class SettingsFromConfig<TSettings> : IGetSettings<TSettings> where TSettings : new()
{
        private readonly string _configPath;

        public SettingsFromConfig(string configPath)
        {
            _configPath = configPath;
        }

        public TSettings GetSettings()
        {
            return JsonConvert.DeserializeObject<TSettings>(File.ReadAllText(_configPath));
        }
}
```

Далее полученный класс можно использовать для получения и внедрения настроек
```csharp
var settings = new SettingsFromConfig<VkSettings>("filename");
var api = new VkBotApiProvider(settings);
```
