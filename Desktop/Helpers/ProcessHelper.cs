using System;
using System.Diagnostics;
using System.Linq;

namespace Helpers
{
    public static class ProcessHelper
    {
        public static void RunPowerShellCommand(string command)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    Verb = "runas",
                    Arguments = command,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "powershell.exe"
                };

                var proc = Process.Start(psi);
                proc.WaitForExit();
                Logging.Logger.WriteLog($"Powershell script '{command}' executed with exit code '{proc.ExitCode}'");
            }
            catch(Exception exception)
            {
                Logging.Logger.WriteLog($"Powershell script '{command}' executed with error '{exception.Message}'");
                throw;
            }

        }

        public static Process RunWinAppDriverProcess()
        {
            KillProcess("WinAppDriver");

            var winAppDriverPath = System.IO.File.Exists(ConfigHelper.WinAppDriverPathX64) ? ConfigHelper.WinAppDriverPathX64 : ConfigHelper.WinAppDriverPathX86;

            try
            {
                var startInfo = new ProcessStartInfo(winAppDriverPath)
                {
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Minimized,
                    Arguments = "127.0.0.1 4723"
                };
                var winAppDriverProcess = Process.Start(startInfo);
                Logging.Logger.WriteLog($"'WinAppDriver' process started");
                return winAppDriverProcess;
            }
            catch (Exception exception)
            {
                Logging.Logger.WriteLog($"Error '{exception.Message}' during WinAppDriver process start in '{winAppDriverPath}'");
                throw;
            }
        }

        public static void KillProcess(string name)
        {
            try
            {
                if (!Process.GetProcessesByName(name).Any()) return;
                Process.GetProcessesByName(name).ToList().ForEach(process =>
                {
                    process.Kill();
                    process.WaitForExit();
                    process.Dispose();
                });
                Logging.Logger.WriteLog($"Process '{name}' killed");
            }
            catch (Exception exception)
            {
                Logging.Logger.WriteLog($"Error '{exception.Message}' during process '{name}' kill");
                throw;
            }

        }

        public static string GetAppVersion(string app)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command (Get-AppxPackage \"{app}\" | Select Version).Version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            var version = process.StandardOutput.ReadToEnd().Trim();

            return string.IsNullOrWhiteSpace(version) ? null : version;
        }


    }
}