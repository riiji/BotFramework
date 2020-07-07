Получение настроек через json конфиг


```csharp
var config = new SettingsFromConfig<TSettings>("filename");
var settings = config.GetSettings()
```

TSettings - класс, в котором описаны настройки бота

Пример такого класса (данный класс доступен из коробки)
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
свойства **должны** быть публичными, также иметь публичные get и set

класс **должен** содержать конструктор без параметров

После этого можно получить настройки
```csharp
var config = new SettingsFromConfig<VkSettings>("filename");
var settings = config.GetSettings()
```
вид JSON конфига для VkSettings

```json
{
"VkKey":"",
"VkAppId":"",
"VkAppSecret":"",
"VkGroupId":""
}
```
