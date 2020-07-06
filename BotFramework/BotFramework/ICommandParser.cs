using BotFramework.Common;

namespace BotFramework.BotFramework
{
    public interface ICommandParser
    {
        CommandArgumentContainer ParseCommand(BotEventArgs botArguments);
    }
}