using System;
using Helpers;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace Extensions
{
    public static class ElementExtension
    {
        public static void ClickElement(this WindowsElement element, string customLog = null)
        {
            customLog ??= $"Click element '{element.GetAttribute("Name")}'";
            element.Click();
            Logging.Logger.WriteLog(customLog);
        }

        public static void ClickElement(this AppiumWebElement element, string customLog = null)
        {
            customLog ??= $"Click element '{element.GetAttribute("Name")}'";
            element.Click();
            Logging.Logger.WriteLog(customLog);
        }

        public static T ClickElement<T>(this WindowsElement element, string customLog = null) where T : new()
        {
            element.ClickElement(customLog);
            return new T();
        }

        public static void EnterText(this WindowsElement element, string text)
        {
            try
            {
                element.ClickElement();
                element.Clear();
                element.SendKeys(text);
                Logging.Logger.WriteLog($"'{text}' text typed into {element.TagName}");
            }
            catch (Exception)
            {
                Logging.Logger.WriteLog($"Error during text entering into {element.TagName}");
            }
        }

        public static void CheckCheckBox(this WindowsElement element, bool check = true)
        {
            if (check != IsCheckboxChecked(element)) element.ClickElement();
            Logging.Logger.WriteLog($"Checkbox '{element.TagName}' checked - {check}");
        }

        public static bool IsCheckboxChecked(this WindowsElement element)
        {
            return element.GetAttribute("Toggle.ToggleState") == "1";
        }

        public static bool IsHighlighted(this WindowsElement element)
        {
            return String.Equals(element.GetAttribute("SelectionItem.IsSelected"), "True");
        }

    }
}