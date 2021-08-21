using System.Linq;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Data.Entities
{
    public class DialogContextEntity
    {
        public int State { get; set; }
        public ContextType ContextType { get; set; }
        public long SenderInfoId { get; set; }

        public static DialogContextEntity GetOrCreate(SenderInfoEntity senderInfoEntity,
            ContextType type,
            BotFrameworkDbContext dbContext)
        {
            DialogContextEntity contextModel = dbContext.DialogContexts.FirstOrDefault(
                c => 
                    c.SenderInfoId == senderInfoEntity.Id 
                    && c.ContextType == ContextType.Discord);

            if (contextModel is null)
            {
                contextModel = new DialogContextEntity()
                {
                    ContextType = type,
                    SenderInfoId = senderInfoEntity.Id
                };

                dbContext.DialogContexts.Add(contextModel);
                dbContext.SaveChanges();
            }

            return contextModel;
        }
    }
}