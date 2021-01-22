using Serilog;
using Serilog.Core;

namespace Tef.BotFramework.Tools.Loggers
{
    public static class LoggerHolder
    {
        //TODO: Lazy<T>
        public static ILogger Instance => _log ??= Create();

        private static ILogger _log;

        public static ILogger Init(ILogger logger) => _log = logger;

        private static ILogger Create()
        {
            Logger log = new LoggerConfiguration()
                //TODO: config level
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                //TODO: config log file path
                .WriteTo.File("bot-framework-log.txt")
                .CreateLogger();
            
            log.Information("[SYS] Start new session");
            return log;
        }
    }
}