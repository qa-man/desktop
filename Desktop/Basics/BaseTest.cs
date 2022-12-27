using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Helpers;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using ScreenRecorderLib;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.UnitTestProvider;
using TestStatus = NUnit.Framework.Interfaces.TestStatus;

namespace Basics;

[Binding]
public sealed class BaseTest
{
    public static string ReportPath;
    private static string ResultFilePath;
    private static string WindowsScreenRecording;
    private static TestContext TestContext;
    private static string TestName;
    private static Recorder VideoRecorder;
    private static string IP;

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        Console.WriteLine("[BeforeTestRun]");
        TestContext = TestContext.CurrentContext;
        PowerShellHelper.DisableFirewall();
        PowerShellHelper.CloseWindowsProxy();
        ProcessHelper.RunWinAppDriverProcess();
        IP = $"{Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)}";
    }

    [BeforeFeature]
    public static void BeforeFeature(FeatureContext featureContext)
    {
        Console.WriteLine("[BeforeFeature]");
        Logging.Logger.WriteLog($"*************************** Feature *{featureContext.FeatureInfo.Title}* starting ************************** \r\n");
    }

    [AfterFeature]
    public static void AfterFeature(FeatureContext featureContext)
    {
        Console.WriteLine("[AfterFeature]");
        Logging.Logger.WriteLog($"*************************** Feature *{featureContext.FeatureInfo.Title}* ended **************************** \r\n\r\n");
    }

    [BeforeScenario]
    public void BeforeScenario(ScenarioContext scenarioContext, FeatureContext featureContext, IUnitTestRuntimeProvider unitTestRuntimeProvider)
    {
        if (scenarioContext.ScenarioInfo.Tags.Any(tag => tag is "Covered"))
        {
            Logging.Logger.WriteLog($"{scenarioContext.ScenarioInfo.Title} --> {scenarioContext.ScenarioInfo.Description}");
            unitTestRuntimeProvider.TestIgnore($"{scenarioContext.ScenarioInfo.Description}");
            return;
        }

        TestContext = TestContext.CurrentContext;
        TestName = System.Text.RegularExpressions.Regex.Replace(TestContext.Test.Name, "[" + System.Text.RegularExpressions.Regex.Escape("? “ ”/ \\ < > * | : \' \"") + "]", "");
        ResultFilePath = Path.Combine(ReportPath, $"·{TestName}·[{DateTime.Now:dd·MMM·yyyy ± HH·mm}]");
        Logging.Logger.WriteLog($"_______________________ Scenario *{scenarioContext.ScenarioInfo.Title}* starting ______________________ \r\n");

        KeyboardHelper.WinD();
        CreateRecording();
    }

    [AfterScenario]
    public void AfterScenario(ScenarioContext scenarioContext)
    {
        TestContext = TestContext.CurrentContext;

        if (TestContext.Result.Outcome.Status is TestStatus.Skipped) return;

        try
        {
            switch (TestContext.Result.Outcome.Status)
            {
                case TestStatus.Passed:
                    var result = TestContext.Result;
                    var currentResult = (TestCaseResult)result.GetType().GetRuntimeFields().SingleOrDefault()?.GetValue(result);
                    var output = currentResult?.Output;

                    var logPath = $"{ResultFilePath}.log";
                    File.WriteAllText(logPath, output);
                    TestContext.AddTestAttachment(logPath);
                    break;

                case TestStatus.Inconclusive:
                case TestStatus.Skipped:
                case TestStatus.Warning:
                case TestStatus.Failed:
                default:
                    var screenshot = $"{ResultFilePath}.png";
                    WindowsHelper.CurrentWindowsSession.GetScreenshot().SaveAsFile(screenshot, ScreenshotImageFormat.Png);
                    TestContext.AddTestAttachment(screenshot);
                    break;
            }

            VideoRecorder.Stop();
            TestContext.AddTestAttachment(WindowsScreenRecording);
        }
        catch (Exception exception)
        {
            Logging.Logger.WriteLog($"'{exception.Message}' error in 'After Scenario' post conditions");
        }
        finally
        {
            WindowsHelper.ConnectIfDisconnected();
        }

        Logging.Logger.WriteLog($"_______________________ Scenario *{scenarioContext.ScenarioInfo.Title}* ended _______________________ \r\n");
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        Console.WriteLine("[AfterTestRun]");
        ProcessHelper.KillProcess("WinAppDriver");
    }

    #region Private Methods

    private static void CreateRecording()
    {
        VideoRecorder = Recorder.CreateRecorder();
        WindowsScreenRecording = $"{ResultFilePath}.mp4";
        VideoRecorder.Record(WindowsScreenRecording);
    }

    #endregion
}