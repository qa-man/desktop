using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Helpers
{
    public static class MethodHelper
    {
        public static bool ExecuteWithTimeLimit(Action method, TimeSpan? timeSpan = null, string log = null)
        {
            timeSpan ??= TimeSpan.FromMinutes(1);
            Func<bool> timeLimitedFunction = () =>
            {
                var task = Task.Factory.StartNew(method);
                task.Wait((TimeSpan) timeSpan);
                return task.IsCompleted;
            };

            return ExecuteSafe(timeLimitedFunction, log);
        }

        public static void ExecuteWithDeadline(Action method, TimeSpan? timeSpan = null, string log = null)
        {
            timeSpan ??= TimeSpan.FromMinutes(1);
            if (ExecuteWithTimeLimit(method, timeSpan, log)) return;
            log ??= $"'{method.Method.Name}' method exceed time limit in '{timeSpan.Value.TotalSeconds}' seconds";
            Logging.Logger.WriteLog(log);
            throw new TimeoutException ($"'{method.Method.Name}' method exceed time limit in '{timeSpan.Value.TotalSeconds}' seconds");
        }

        public static bool ExecuteUntilCondition(Func<bool> method, bool expectedCondition = true, double timeLimitInSeconds = 30, string customLog = null)
        {
            var time = new Stopwatch();
            time.Start();
            var result = method.Invoke();

            while (result != expectedCondition)
            {
                result = method.Invoke();
                if (!(time.Elapsed.TotalSeconds >= TimeSpan.FromSeconds(timeLimitInSeconds).TotalSeconds)) continue;
                customLog = $"'{method.Method.Name}' method execution time limit exceeded - '{time.Elapsed.TotalSeconds}'";
                break;
            }

            customLog ??= $"'{method.Method.Name}' method executed in '{time.Elapsed.TotalSeconds}' seconds";
            Logging.Logger.WriteLog(customLog);

            return result == expectedCondition;
        }

        public static T ExecuteSafe<T>(Func<T> method, string log = null)
        {
            log ??= $"'{method.Method.Name}' method executed";
            try
            {
                var result = method.Invoke();
                Logging.Logger.WriteLog(log);
                return result;
            }
            catch (Exception exception)
            {
                Logging.Logger.WriteLog($"'{exception.Message}' during '{method.Method.Name}' execution");
                throw exception.GetBaseException();
            }
        }

        public static void ExecuteSafe(Action method, string log = null)
        {
            log ??= $"'{method.Method.DeclaringType?.FullName} - {method.Method.Name}' method executed";
            try
            {
                method.Invoke();
                Logging.Logger.WriteLog(log);
            }
            catch (Exception exception)
            {
                Logging.Logger.WriteLog($"'{exception.Message}' during '{method.Method.Name}' execution");
                throw exception.GetBaseException();
            }
        }
    }
}