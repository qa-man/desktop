using System;
using Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Extensions
{
    public static class DriverExtension
    {
        public static WindowsElement GetElement(this WindowsDriver<WindowsElement> windowsApp, By locator, double timeLimitInSeconds = 30)
        {
            return windowsApp.WaitElementToBeDisplayed(locator, timeLimitInSeconds).FindElement(locator);
        }

        public static WindowsDriver<WindowsElement> WaitElementToBeDisplayed(this WindowsDriver<WindowsElement> windowsApp, By locator, double timeLimitInSeconds = 30)
        {
            if (IsElementDisplayed(windowsApp, locator, timeLimitInSeconds)) return windowsApp;
            Logging.Logger.WriteLog($"Element with locator '{locator}' has NOT been found.");
            throw new NotFoundException($"Element with locator '{locator}' has NOT been found.");
        }

        public static bool WaitElementToBeNotDisplayed(this WindowsDriver<WindowsElement> windowsApp, By locator, double timeLimitInSeconds = 30)
        {

            bool ElementIsVisible()
            {
                try
                {
                    return WaitElementCondition(windowsApp, ExpectedConditions.ElementIsVisible(locator), timeLimitInSeconds: 1);
                }
                catch (WebDriverTimeoutException)
                {
                    return false;
                }
            }

            var outcome = MethodHelper.ExecuteUntilCondition(ElementIsVisible, expectedCondition: false);

            var log = outcome
                            ? $"Element with locator '{locator}' disappeared as expected"
                            : $"Element with locator '{locator}' still displayed after '{timeLimitInSeconds}' seconds but should NOT";
            Logging.Logger.WriteLog(log);

            return outcome;
        }

        public static bool IsElementDisplayed(this WindowsDriver<WindowsElement> windowsApp, By locator, double timeLimitInSeconds = 1)
        {
            try
            {
                var result = IsElementExist(windowsApp, locator, timeLimitInSeconds) && WaitElementCondition(windowsApp, ExpectedConditions.ElementToBeClickable(locator), timeLimitInSeconds);
                Logging.Logger.WriteLog($"Element with locator '{locator}' displayed.");
                return result;
            }
            catch (WebDriverTimeoutException)
            {
                Logging.Logger.WriteLog($"Element with locator '{locator}' does NOT displayed.");
                return false;
            }
        }

        public static bool IsElementDisabled(this WindowsDriver<WindowsElement> windowsApp, By locator, double timeLimitInSeconds = 1)
        {
            try
            {
                var result = IsElementExist(windowsApp, locator, timeLimitInSeconds) && !windowsApp.FindElement(locator).Enabled;
                Logging.Logger.WriteLog($"Element with locator '{locator}' exist and disabled.");
                return result;
            }
            catch (WebDriverTimeoutException)
            {
                Logging.Logger.WriteLog($"Element with locator '{locator}' does NOT exist.");
                return false;
            }
        }

        public static void DoubleClick(this WindowsDriver<WindowsElement> windowsApp, WindowsElement element)
        {
            windowsApp.Mouse.MouseMove(element.Coordinates);
            windowsApp.Mouse.DoubleClick(element.Coordinates);
            Logging.Logger.WriteLog($"Element '{element}' double clicked.");
        }

        public static void ClickElementUsingAction(this WindowsDriver<WindowsElement> windowsApp, WindowsElement element, string customLog = null)
        {
            customLog ??= $"Click element '{element.GetAttribute("Name")}'";
            new Actions(windowsApp).MoveToElement(element, element.Size.Width, element.Size.Height).Click().Build().Perform();
            Logging.Logger.WriteLog(customLog);
        }

        public static void ClickElementUsingAction(this WindowsDriver<WindowsElement> windowsApp, AppiumWebElement element, string customLog = null)
        {
            customLog ??= $"Click element '{element.GetAttribute("Name")}'";
            new Actions(windowsApp).MoveToElement(element, element.Size.Width, element.Size.Height).Click().Build().Perform();
            Logging.Logger.WriteLog(customLog);
        }

        #region Private Methods

        private static bool IsElementExist(this WindowsDriver<WindowsElement> windowsApp, By locator, double timeLimitInSeconds = 1)
        {
            try
            {
                WaitElementCondition(windowsApp, ExpectedConditions.ElementExists(locator), timeLimitInSeconds);
            }
            catch (WebDriverTimeoutException)
            {
                Logging.Logger.WriteLog($"Element with locator '{locator}' does NOT exist.");
                return false;
            }
            Logging.Logger.WriteLog($"Element with locator '{locator}' exists.");
            return true;
        }

        private static bool WaitElementCondition(WindowsDriver<WindowsElement> windowsApp, Func<IWebDriver, IWebElement> condition, double timeLimitInSeconds = 1)
        {
            var wait = new WebDriverWait(windowsApp, TimeSpan.FromSeconds(timeLimitInSeconds))
            {
                PollingInterval = TimeSpan.FromMilliseconds(100)
            };
            wait.IgnoreExceptionTypes(typeof(WebDriverException));
            return wait.Until(condition) is not null;
        }

        #endregion

    }
}