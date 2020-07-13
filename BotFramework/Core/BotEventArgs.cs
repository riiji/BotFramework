using System;

namespace Tef.BotFramework.Core
{
    public class BotEventArgs : EventArgs
    {
        public BotEventArgs(string text, long groupId, int userSenderId)
        {
            Text = text;
            GroupId = groupId;
            UserSenderId = userSenderId;
        }

        public string Text { get; }

        public long GroupId { get; }

        public int UserSenderId { get; }
    }
}