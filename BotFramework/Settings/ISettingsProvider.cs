namespace Tef.BotFramework.Settings
{
    public interface ISettingsProvider<out TSettings>
    {
        TSettings GetSettings();
    }
}