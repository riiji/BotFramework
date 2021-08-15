using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Kysect.BotFramework.ApiProviders;
using Telegram.Bot.Requests;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotPollMessage : IBotMessage
    {
        public BotPollMessage(string text)
        {
            Text = text;
        }

        public string Text { get; }

        public void Send(IBotApiProvider apiProvider, SenderInfo sender)
        {
            apiProvider.SendPollMessage(Text, sender);
        }
    }
}