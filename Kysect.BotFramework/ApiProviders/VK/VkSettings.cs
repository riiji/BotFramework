namespace Kysect.BotFramework.ApiProviders.VK
{
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
}