using System.Windows.Forms;
using Extensions;
using WindowsInput;

namespace Helpers
{
    public static class KeyboardHelper
    {
        private static IKeyboardSimulator Keyboard = new InputSimulator().Keyboard;

        public static void WinD()
        {
            Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_D);
        }

        public static void PressKey(Keyboard key)
        {
            SendKeys.SendWait(key.GetValue());
        }

    }

    public enum Keyboard
    {
        [Value("{TAB}")]
        Tab,
        [Value(" ")]
        Space,
        [Value("{ENTER}")]
        Enter,
        [Value("{DOWN}")]
        Down,
        [Value("{UP}")]
        Up,
        [Value("{LEFT}")]
        Left,
        [Value("{RIGHT}")]
        Right,
        [Value("+")]
        Shift,
        [Value("{ESC}")]
        Escape
    }
}