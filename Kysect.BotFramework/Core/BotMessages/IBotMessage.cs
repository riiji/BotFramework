using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Core.BotMessages
{
    public interface IBotMessage
    {
        string Text { get; }

        void Send(IBotApiProvider apiProvider, SenderInfo sender);
    }
}