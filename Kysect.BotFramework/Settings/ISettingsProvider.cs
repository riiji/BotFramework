namespace Kysect.BotFramework.Settings
{
    public interface ISettingsProvider<out TSettings>
    {
        TSettings GetSettings();
    }
}