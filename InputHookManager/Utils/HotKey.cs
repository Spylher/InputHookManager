using InputHookManager.Enums;

namespace InputHookManager.Utils
{
    public class HotKey
    {
        public bool CtrlKeyPressed { get; set; }
        public bool ShiftKeyPressed { get; set; }
        public bool AltKeyPressed { get; set; }
        public KeyInput MainKey { get; set; } = KeyInput.None;

        public HotKey(KeyInput mainKey = KeyInput.None, bool ctrlKeyPressed = false, bool shiftKeyPressed = false, bool altKeyPressed = false)
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

            MainKey = (KeyInput)Enum.Parse(typeof(KeyInput), text, true);
        }

        public void Clear()
        {
            CtrlKeyPressed = false;
            AltKeyPressed = false;
            ShiftKeyPressed = false;
            MainKey = KeyInput.None;
        }

        public override string ToString()
        {
            var invalidKeys = new List<KeyInput> { KeyInput.Back, KeyInput.Space, KeyInput.Capital, KeyInput.LWin, KeyInput.RWin, KeyInput.Escape, KeyInput.Delete, KeyInput.None };
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
