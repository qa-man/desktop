using System;
using System.IO;
using Basics;

namespace Helpers
{
    public class Logging
    {
        #region Logger

        public static string LogFolderPath => $"{BaseTest.ReportPath}\\Log\\";

        private Logging() { }
        private static object _logLock;
        private static Logging _instance;
        private static string _logFileName;

        public static Logging Logger
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new Logging();
                _logLock = new object();
                _logFileName = Guid.NewGuid() + ".log";
                return _instance;
            }
        }

        #endregion

        public void WriteLog(string logContent, LogType logType = LogType.All, string fileName = null) 
        {
            try
            {
                Directory.CreateDirectory($"{LogFolderPath}");
                fileName ??= _logFileName;

                string[] logText = {$"{DateTime.Now:hh:mm:ss}: {logType}: {logContent}"};

                lock (_logLock) File.AppendAllLines($"{LogFolderPath}{fileName}", logText);
            }
            catch (Exception e) { Console.WriteLine($"{e.Message}"); }
        }
    }

    public enum LogType
    {
        All,
        Information,
        Debug,
        Success,
        Failure,
        Warning,
        Error
    }
}
