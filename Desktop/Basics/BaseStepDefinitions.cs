using Helpers;
using OpenQA.Selenium.Appium.Windows;
using TechTalk.SpecFlow;

namespace Basics
{
    [Binding]
    public class BaseStepDefinitions
    {
        [Given(@"Pre-conditions example")]
        public static WindowsDriver<WindowsElement> Preconditions() => WindowsHelper.CurrentWindowsSession;

        [When(@"Example")]
        public static void GivenExample()
        {
            // Example
        }

        [Then(@"Example")]
        public static void ThenExample()
        {
            // Example
        }
        
    }
}