using System;
using System.Collections.Generic;
using System.Linq;
using Kysect.BotFramework.Core.CommandInvoking;

namespace Kysect.BotFramework.Core
{
    public class BotEventArgs : EventArgs
    {
        public BotEventArgs(BotMessage message, long groupId, long userSenderId, string username)
        {
            Message = message;
            GroupId = groupId;
            UserSenderId = userSenderId;
            Username = username;
        }

        public BotEventArgs(BotMessage message, CommandArgumentContainer commandWithArgs) : this(message, commandWithArgs.Sender)
        {
        }

        public BotEventArgs(BotMessage message, BotEventArgs sender)
            : this(message, sender.GroupId, sender.UserSenderId, sender.Username)
        {
        }

        public BotMessage Message { get; }
        public long GroupId { get; }
        public long UserSenderId { get; }
        public string Username { get; }

        public string FindCommandName()
        {
            //TODO: add separator list
            return Message.Text.Split().FirstOrDefault();
        }

        public List<string> GetCommandArguments()
        {
            return Message.Text.Split().Skip(1).ToList();
        }
    }
}