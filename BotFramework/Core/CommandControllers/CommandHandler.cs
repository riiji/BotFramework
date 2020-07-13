using System.Linq;
using System.Runtime.CompilerServices;
using Tef.BotFramework.Abstractions;
using Tef.BotFramework.Common;

namespace Tef.BotFramework.Core.CommandControllers
{
    public class CommandHandler
    {
        private readonly CommandsList _commands;

        public CommandHandler(CommandsList commands)
        {
            _commands = commands;
        }

        public Result IsCommandCorrect(CommandArgumentContainer args)
        {
            var commandTask = _commands.GetCommand(args.CommandName.ToLower());

            if (!commandTask.IsSuccess)
                return commandTask;

            var command = commandTask.Value;

            if (command.CanExecute(args))
                return new Result(true,
                    $"command {args.CommandName} can be executable with args {string.Join(' ', args.Arguments.Select(x => x))}");

            var loggerMessage =
                $"command {command.CommandName} not executable with args {string.Join(' ', args.Arguments.Select(x => x))}";
            //loggerMessage = args.Arguments.Aggregate(loggerMessage, (current, t) => current + (t + " "));

            return new Result(false, loggerMessage);
        }

        public void RegisterCommand(IBotCommand command)
        {
            _commands.AddCommand(command);
        }

        public Result ExecuteCommand(CommandArgumentContainer args)
        {
            var commandTask = _commands.GetCommand(args.CommandName.ToLower());

            if (!commandTask.IsSuccess)
                return commandTask;

            var command = commandTask.Value;
            var commandExecuteResult = command.Execute(args);

            return commandExecuteResult;
        }

        public CommandsList GetCommands()
        {
            return _commands;
        }
    }
}