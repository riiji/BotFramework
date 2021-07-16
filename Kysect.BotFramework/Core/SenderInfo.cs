using Telegram.Bot.Types;

namespace Kysect.BotFramework.Core
{
    public class SenderInfo
    {
        public long GuildId { get; }
        public long ChatId { get; }
        public long UserSenderId { get; }
        public string Username { get; }

        public bool IsAdmin { get; }

        public SenderInfo(long guildId, long chatId, long userSenderId, string username, bool isAdmin)
        {
            GuildId = guildId;
            ChatId = chatId;
            UserSenderId = userSenderId;
            Username = username;
            IsAdmin = isAdmin;
        }
    }
}