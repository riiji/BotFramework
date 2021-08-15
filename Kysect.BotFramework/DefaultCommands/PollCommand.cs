using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Commands;

namespace Kysect.BotFramework.DefaultCommands
{
    public class PollCommand : IBotAsyncCommand
    {
        public static readonly BotCommandDescriptor<PollCommand> Descriptor = new BotCommandDescriptor<PollCommand>(
            "Poll", 
            "Create a new poll", new List<string>());

        public Result CanExecute(CommandContainer args) => Result.Ok();

        public Task<Result<IBotMessage>> Execute(CommandContainer args)
        {
            IBotMessage message = new BotPollMessage(String.Join(" ", args.Arguments));
            return Task.FromResult(Result.Ok(message));
        }
    }
}