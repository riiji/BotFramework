Получение настроек через sqlite


```csharp
var settings = new SettingsFromSqlite<VkSettings>("filename");
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
var settings = new SettingsFromConfig<VkSettings>("filename");
```
в sqlite файле должна находится таблица BotSettings

эта таблица представляет из себя пару ключ-значение для получения настроек

для VkSettings в таблице BotSettings должны находится такие пары значений:

[VkKey, key]

[VkAppId, id]

[VkAppSecret, secret]

[VkGroupId, groupId]
