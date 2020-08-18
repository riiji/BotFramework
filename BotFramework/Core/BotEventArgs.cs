using System;

namespace Tef.BotFramework.Core
{
    public class BotEventArgs : EventArgs
    {
        public BotEventArgs(string text, long groupId, int userSenderId, string username)
        {
            Text = text;
            GroupId = groupId;
            UserSenderId = userSenderId;
            Username = username;
        }
        public string Text { get; }
        public long GroupId { get; }
        public int UserSenderId { get; }
        public string Username { get; }
    }
}