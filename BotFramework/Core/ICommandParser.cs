namespace Tef.BotFramework.Core
{
    public interface ICommandParser
    {
        CommandArgumentContainer ParseCommand(BotEventArgs botArguments);
    }
}