using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Helpers
{
    public static class ConfigHelper
    {
        public static IConfigurationRoot Config = new ConfigurationBuilder().SetBasePath(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName).AddJsonFile("config.json").Build();

        public static string PackageId => Config["PackageID"];
        public static string AppId => $"{Config["PackageID"]}_{Config["PublisherID"]}!App";
        public static string AdbPath => $"{Environment.GetEnvironmentVariable("ANDROID_HOME")}\\platform-tools\\adb.exe";
        public static string WinAppDriverUrl => Config["WindowsApplicationDriverUrl"];
        public static string WinAppDriverPathX64 => Config["WindowsApplicationDriverPathX64"];
        public static string WinAppDriverPathX86 => Config["WindowsApplicationDriverPathX86"];

    }
}