using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Tef.BotFramework.Settings
{
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
}