using System;
using System.Linq;
using System.Threading;
using ManagedNativeWifi;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace Helpers
{
    public static class WindowsHelper
    {
        public static WindowsDriver<WindowsElement> CurrentWindowsSession { get { if (windowsSession is null) GetWindowsSession(); return windowsSession; } }
        
        private static WindowsDriver<WindowsElement> windowsSession;
        private static InterfaceConnectionInfo connectedNetworkInterface = NativeWifi.EnumerateInterfaceConnections().Single(network => network.IsConnected);
        private static bool disconnected;

        public static WindowsDriver<WindowsElement> GetWindowsSession()
        {
            try
            {
                var browserWindowOptions = new AppiumOptions();
                browserWindowOptions.AddAdditionalCapability("app", "Root");
                browserWindowOptions.AddAdditionalCapability("ms:waitForAppLaunch", "30");
                browserWindowOptions.AddAdditionalCapability("deviceName", "WindowsPC");
                browserWindowOptions.AddAdditionalCapability("platformName", "Windows");
                browserWindowOptions.AddAdditionalCapability("ms:experimental-webdriver", true);
                browserWindowOptions.SetLoggingPreference(OpenQA.Selenium.LogType.Server, LogLevel.All);

                windowsSession = new WindowsDriver<WindowsElement>(new Uri(ConfigHelper.WinAppDriverUrl), browserWindowOptions);
                Logging.Logger.WriteLog($"WindowsDriver session with '{windowsSession.SessionId}' session id started");
                return windowsSession;
            }
            catch (Exception exception)
            {
                Logging.Logger.WriteLog($"Error '{exception.Message}' during WindowsDriver process started");
                throw;
            }
        }

        public static void TurnOffWiFi(InterfaceConnectionInfo networkInterface = null)
        {
            networkInterface ??= connectedNetworkInterface;

            try
            {
                var log = NativeWifi.TurnOffInterfaceRadio(networkInterface.Id)
                                ? $"'{networkInterface.ProfileName}' network interface switched OFF"
                                : $"'{networkInterface.ProfileName}' network interface can NOT be switched OFF";
                Logging.Logger.WriteLog(log);
                MethodHelper.ExecuteUntilCondition(() => NativeWifi.EnumerateInterfaceConnections().Any(network => network.IsConnected), expectedCondition: false);
                disconnected = true;
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
            catch (Exception exception)
            {
                Logging.Logger.WriteLog($"'{exception.Message}' error during '{networkInterface.ProfileName}' network interface switch OFF");
                throw;
            }

        }

        public static void TurnOnWiFi(InterfaceConnectionInfo networkInterface = null)
        {
            networkInterface ??= connectedNetworkInterface;

            try
            {
                if (NativeWifi.EnumerateInterfaceConnections().Any(network => network.IsConnected)
                    && NativeWifi.EnumerateInterfaceConnections().Single(network => network.IsConnected).Id.Equals(networkInterface.Id))
                {
                    Logging.Logger.WriteLog($"'Turn On Network' method executing but the network '{networkInterface.ProfileName}' already connected");
                    disconnected = false;
                    return;
                }

                var log = NativeWifi.TurnOnInterfaceRadio(networkInterface.Id)
                    ? $"'{networkInterface.ProfileName}' network interface switched ON"
                    : $"'{networkInterface.ProfileName}' network interface can NOT be switched ON";
                Logging.Logger.WriteLog(log);
                MethodHelper.ExecuteUntilCondition(() => NativeWifi.EnumerateInterfaceConnections().Any(network => network.IsConnected));
                disconnected = false;
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            catch (Exception exception)
            {
                Logging.Logger.WriteLog($"'{exception.Message}' error during '{networkInterface.ProfileName}' network interface switch ON");
                throw;
            }
        }

        public static void ConnectIfDisconnected()
        {
            if (disconnected) TurnOnWiFi();
        }

    }
}