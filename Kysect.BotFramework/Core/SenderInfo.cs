namespace Kysect.BotFramework.Core
{
    public class SenderInfo
    {
        public long GroupId { get; }
        public long UserSenderId { get; }
        public string Username { get; }

        public SenderInfo(long groupId, long userSenderId, string username)
        {
            GroupId = groupId;
            UserSenderId = userSenderId;
            Username = username;
        }
    }
}