using Tef.BotFramework.Common;

namespace Tef.BotFramework.BotFramework
{
    public interface ICommandParser
    {
        CommandArgumentContainer ParseCommand(BotEventArgs botArguments);
    }
}