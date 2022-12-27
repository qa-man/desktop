using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Helpers
{
    public static class PowerShellHelper
    {
        public static void RunPowerShellCommand(string script)
        {
            try
            {
                using var runSpace = RunspaceFactory.CreateRunspace();
                runSpace.Open();
                var ps = PowerShell.Create();
                ps.AddScript(script);
                ps.Invoke();
                Logging.Logger.WriteLog($"PowerShell script '{script}' executed");

            }
            catch (Exception exception)
            {
                Logging.Logger.WriteLog($"PowerShell script '{script}' executed with error '{exception.Message}'");
                throw;
            }
        }

        public static string RunPowershellCommand(string command)
        {
            string runResult = "";
            try
            {
                Process process = new Process();
                process.StartInfo.WorkingDirectory = @"C:/WINDOWS/system32";
                process.StartInfo.FileName = @"powershell.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.Verb = "runas";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Arguments = command;
                process.Start();
                runResult = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                return runResult;
            }
            catch (Exception ex)
            {
                runResult = $"RunCmd({command}) catch an exception => {ex}";
                return runResult;
            }

        }

        public static void DisableFirewall()
        {
            RunPowerShellCommand($"Netsh advfirewall set allprofile state off");
        }

        public static void CloseWindowsProxy()
        {
            RunPowerShellCommand("Set-ItemProperty -Path \"Registry::HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\" ProxyEnable -value 0");
        }

    }
}
