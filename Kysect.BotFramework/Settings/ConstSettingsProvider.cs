namespace Kysect.BotFramework.Settings
{
    public class ConstSettingsProvider<TSettings> : ISettingsProvider<TSettings>
    {
        private readonly TSettings _settings;

        public ConstSettingsProvider(TSettings settings)
        {
            _settings = settings;
        }

        public TSettings GetSettings()
        {
            return _settings;
        }
    }
}