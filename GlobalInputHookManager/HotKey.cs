using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GlobalInputHookManager
{
    public class HotKey
    {
        public bool CtrlKeyPressed { get; set; }
        public bool ShiftKeyPressed { get; set; }
        public bool AltKeyPressed { get; set; }
        public Keys MainKey { get; set; }

        public HotKey(Keys mainKey = Keys.None, bool ctrlKeyPressed = false, bool shiftKeyPressed = false, bool altKeyPressed = false)
        {
            CtrlKeyPressed = ctrlKeyPressed;
            ShiftKeyPressed = shiftKeyPressed;
            AltKeyPressed = altKeyPressed;
            MainKey = mainKey;
        }

        public static HotKey GetByText(string text)
        {
            var hotKey = new HotKey();

            if (text.Contains("Ctrl+"))
            {
                hotKey.CtrlKeyPressed = true;
                text = text.Replace("Ctrl+", "");
            }
            if (text.Contains("Shift+"))
            {
                hotKey.ShiftKeyPressed = true;
                text = text.Replace("Shift+", "");
            }
            if (text.Contains("Alt+"))
            {
                hotKey.AltKeyPressed = true;
                text = text.Replace("Alt+", "");
            }


            if (!string.IsNullOrEmpty(text))
            {
                Keys key = (Keys)Enum.Parse(typeof(Keys), text, true);
                hotKey.MainKey = key;
            }

            return hotKey;
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
            var invalidKeys = new List<Keys>{ Keys.Back, Keys.Space, Keys.Capital, Keys.LWin, Keys.RWin, Keys.Escape, Keys.Delete, Keys.None };
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
            if (obj is HotKey otherClass)
                return GetHashCode() == otherClass.GetHashCode();

            return false;
        }

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(MainKey, CtrlKeyPressed, ShiftKeyPressed, AltKeyPressed);
        //}

        public static implicit operator string(HotKey hotKey)
        {
            return hotKey?.ToString();
        }
    }
}
