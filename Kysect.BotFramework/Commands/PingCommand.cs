using System.Threading.Tasks;
using FluentResults;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.CommandInvoking;

namespace Kysect.BotFramework.Commands
{
    public class PingCommand : IBotAsyncCommand
    {
        public static readonly BotCommandDescriptor<PingCommand> Descriptor = new BotCommandDescriptor<PingCommand>(
            "Ping",
            "Answer pong on ping message");

        public Result CanExecute(CommandArgumentContainer args)
        {
            return Result.Ok();
        }

        public Task<Result<IBotMessage>> Execute(CommandArgumentContainer args)
        {
            IBotMessage message = new BotSingleMediaMessage("Hi!", new BotOnlinePhotoFile("https://www.ferra.ru/thumb/1800x0/filters:quality(75):no_upscale()/imgs/2020/12/09/06/4390537/dd4dfcc4320bfd0c1d7748783337cd825acd4674.jpg"));
            return Task.FromResult(Result.Ok(message));
        }
    }
}