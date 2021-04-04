using System.Threading.Tasks;

namespace Kysect.BotFramework.Core.Tools.Extensions
{
    public static class TaskSafeWaitExtensions
    {
        public static void WaitSafe(this Task task)
        {
            Task.WaitAny(task);
        }

        public static void WaitSafe<T>(this Task<T> task)
        {
            Task.WaitAny(task);
        }
    }
}