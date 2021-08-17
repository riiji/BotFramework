using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Data.Entities
{
    public class DialogContextEntity
    {
        public int State { get; set; }
        public ContextType ContextType { get; set; }
        public long SenderInfoId { get; set; }
    }
}