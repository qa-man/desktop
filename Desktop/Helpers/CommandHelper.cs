using System;
using System.Diagnostics;

namespace Helpers
{
    public static class CommandHelper
    {
        public static void RunCmdCommand(string cmdStr, string log = null)
        {
            try
            {
                var processInfo = new ProcessStartInfo("cmd.exe", "/S /C " + cmdStr)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Verb = "runas",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                var process = new Process
                {
                    StartInfo = processInfo
                };
                process.Start();
                process.WaitForExit();
                process.Close();
                log ??= $"'{processInfo.Arguments}' command executed";
                Logging.Logger.WriteLog(log);
            }
            catch (Exception ex)
            {
                Logging.Logger.WriteLog($"'{cmdStr}' command execution throws exception - '{ex.Message}'", LogType.Error);
            }
        }
    }

}
