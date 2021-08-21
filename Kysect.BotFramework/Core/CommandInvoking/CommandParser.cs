using FluentResults;
using Kysect.BotFramework.Core.Commands;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandParser : ICommandParser
    {
        public Result<CommandContainer> ParseCommand(BotEventArgs botArguments)
        {
            string commandName = botArguments.FindCommandName();

            if (string.IsNullOrWhiteSpace(commandName))
            {
                return Result.Fail($"[{nameof(CommandParser)}]: Message do not contains command name.");
            }

            return Result.Ok(new CommandContainer(commandName, botArguments.Context, botArguments.GetCommandArguments(),
                                                  botArguments.GetMediaFiles()));
        }
    }
}