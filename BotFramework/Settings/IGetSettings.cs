namespace Tef.BotFramework.Settings
{
    public interface IGetSettings<out TSettings>
    {
        TSettings GetSettings();
    }
}