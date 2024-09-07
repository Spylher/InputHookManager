using System;
using System.Collections.Generic;
//using System.Windows.Forms;
using GlobalInputHookManager.Enums;

namespace GlobalInputHookManager
{
    public class HotKey
    {
        public bool CtrlKeyPressed { get; set; }
        public bool ShiftKeyPressed { get; set; }
        public bool AltKeyPressed { get; set; }
        public Keys MainKey { get; set; } = Keys.None;

        public HotKey(Keys mainKey = Keys.None, bool ctrlKeyPressed = false, bool shiftKeyPressed = false, bool altKeyPressed = false)
        {
            CtrlKeyPressed = ctrlKeyPressed;
            ShiftKeyPressed = shiftKeyPressed;
            AltKeyPressed = altKeyPressed;
            MainKey = mainKey;
        }

        public HotKey(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (text.Contains("Ctrl+"))
            {
                CtrlKeyPressed = true;
                text = text.Replace("Ctrl+", "");
            }
            if (text.Contains("Shift+"))
            {
                ShiftKeyPressed = true;
                text = text.Replace("Shift+", "");
            }
            if (text.Contains("Alt+"))
            {
                AltKeyPressed = true;
                text = text.Replace("Alt+", "");
            }

            MainKey = (Keys)Enum.Parse(typeof(Keys), text, true);
        }

        public void Clear()
        {
            CtrlKeyPressed = false;
            AltKeyPressed = false;
            ShiftKeyPressed = false;
            MainKey = Keys.None;
        }

        public override string ToString()
        {
            var invalidKeys = new List<Keys> { Keys.Back, Keys.Space, Keys.Capital, Keys.LWin, Keys.RWin, Keys.Escape, Keys.Delete, Keys.None };
            var shortcut = string.Empty;

            if (CtrlKeyPressed && ShiftKeyPressed)
                shortcut += "Ctrl+Shift+";
            else if (CtrlKeyPressed && AltKeyPressed)
                shortcut += "Ctrl+Alt+";
            else if (ShiftKeyPressed && AltKeyPressed)
                shortcut += "Alt+Shift+";
            else if (CtrlKeyPressed)
                shortcut += "Ctrl+";
            else if (ShiftKeyPressed)
                shortcut += "Shift+";
            else if (AltKeyPressed)
                shortcut += "Alt+";
            if (invalidKeys.Contains(MainKey))
                shortcut = "None";
            else
                shortcut += MainKey.ToString();

            return shortcut;
        }

        public override bool Equals(object obj)
        {
            if (obj is HotKey otherHk)
                return GetHashCode() == otherHk.GetHashCode();

            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static implicit operator string(HotKey hotKey)
        {
            return hotKey?.ToString();
        }
    }
}
