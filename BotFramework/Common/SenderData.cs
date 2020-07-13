namespace Tef.BotFramework.Common
{
    public class SenderData
    {
        public SenderData(long groupId)
        {
            GroupId = groupId;
        }

        public long GroupId { get; }
    }
}