namespace Kysect.BotFramework.Core
{
    public class SenderInfo
    {
        public long GroupId { get; }
        public long UserSenderId { get; }
        public string Username { get; }

        public bool IsAdmin { get; }

        public SenderInfo(long groupId, long userSenderId, string username, bool isAdmin)
        {
            GroupId = groupId;
            UserSenderId = userSenderId;
            Username = username;
            IsAdmin = isAdmin;
        }
    }
}