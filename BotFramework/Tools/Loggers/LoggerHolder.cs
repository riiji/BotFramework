using Serilog;
using Serilog.Core;

namespace Tef.BotFramework.Tools.Loggers
{
    public static class LoggerHolder
    {
        public static ILogger Instance => _log ??= Create();

        private static ILogger _log;

        public static ILogger Init(ILogger logger) => _log = logger;

        private static ILogger Create()
        {
            Logger log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File("bot-framework-log.txt")
                .CreateLogger();
            
            log.Information("[SYS] Start new session");
            return log;
        }
    }
}