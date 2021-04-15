using FluentResults;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandParser : ICommandParser
    {
        public Result<CommandArgumentContainer> ParseCommand(BotEventArgs botArguments)
        {
            string commandName = botArguments.FindCommandName();

            if (string.IsNullOrWhiteSpace(commandName))
                return Result.Fail($"[{nameof(CommandParser)}]: Message do not contains command name.");

            return Result.Ok(new CommandArgumentContainer(commandName, botArguments.Sender, botArguments.GetCommandArguments()));
        }
    }
}