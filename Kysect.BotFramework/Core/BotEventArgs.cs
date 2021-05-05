using System;
using System.Collections.Generic;
using System.Linq;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.CommandInvoking;

namespace Kysect.BotFramework.Core
{
    public class BotEventArgs : EventArgs
    {
        public BotEventArgs(IBotMessage message, SenderInfo sender)
        {
            Message = message;
            Sender = sender;
        }

        public BotEventArgs(IBotMessage message, CommandArgumentContainer commandWithArgs) : this(message, commandWithArgs.Sender)
        {
        }

        public IBotMessage Message { get; }

        public SenderInfo Sender { get; }

        public string FindCommandName()
        {
            //TODO: add separator list
            return Message.Text.Split().FirstOrDefault();
        }

        public List<string> GetCommandArguments()
        {
            return Message.Text.Split().Skip(1).ToList();
        }

        public List<IBotMediaFile> GetMediaFiles()
        {
            if (Message is BotSingleMediaMessage singleMediaMessage)
            {
                return new List<IBotMediaFile>() {singleMediaMessage.MediaFile};
            } else if (Message is BotMultipleMediaMessage multipleMediaMessage)
            {
                return multipleMediaMessage.MediaFiles;
            }

            return new List<IBotMediaFile>();
        }
    }
}