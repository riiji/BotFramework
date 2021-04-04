using System;
using System.Collections.Generic;
using System.Linq;
using Kysect.BotFramework.Core.CommandInvoking;

namespace Kysect.BotFramework.Core
{
    public class BotEventArgs : EventArgs
    {
        public BotEventArgs(string text, long groupId, long userSenderId, string username)
        {
            Text = text;
            GroupId = groupId;
            UserSenderId = userSenderId;
            Username = username;
        }

        public BotEventArgs(string text, CommandArgumentContainer commandWithArgs) : this(text, commandWithArgs.Sender)
        {
        }

        public BotEventArgs(string text, BotEventArgs sender)
            : this(text, sender.GroupId, sender.UserSenderId, sender.Username)
        {
        }

        public string Text { get; }
        public long GroupId { get; }
        public long UserSenderId { get; }
        public string Username { get; }

        public string FindCommandName()
        {
            //TODO: add separator list
            return Text.Split().FirstOrDefault();
        }

        public List<string> GetCommandArguments()
        {
            return Text.Split().Skip(1).ToList();
        }
    }
}