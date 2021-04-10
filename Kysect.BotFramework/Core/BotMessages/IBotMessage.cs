using Kysect.BotFramework.ApiProviders;

namespace Kysect.BotFramework.Core.BotMessages
{
    public interface IBotMessage
    {
        string Text { get; }

        void Send(IBotApiProvider apiProvider,BotEventArgs sender);
    }
}