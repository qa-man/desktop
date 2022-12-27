using Extensions;
using Helpers;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace Basics
{
    public class BasePage
    {
        protected static WindowsDriver<WindowsElement> ExampleSession => WindowsHelper.CurrentWindowsSession;

        public WindowsElement ElementExample => ExampleSession.GetElement(MobileBy.AccessibilityId("example"));

        public void ClickExampleElement()
        {
            ElementExample.ClickElement();
        }
    }
}