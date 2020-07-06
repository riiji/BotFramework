using System;
using Tef.BotFramework.Database;

namespace Tef.BotFramework.VK
{
    class VkSettingsFromDatabase : IGetVkSettings
    {
        public VkSettings GetVkSettings()
        {
            using var db = new DatabaseContext();
            var settings = db.BotSettings;

            var key = settings.Find("VkKey").Value;
            var appId = Convert.ToInt32(settings.Find("VkAppId").Value);
            var appSecret = settings.Find("VkAppSecret").Value;
            var groupId = Convert.ToInt32(settings.Find("VkGroupId").Value);

            return new VkSettings(key, appId, appSecret, groupId);
        }
    }
}