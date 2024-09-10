using InputHookManager.Enums;

namespace InputHookManager.Utils
{
    public class HotKey
    {
        public bool CtrlKeyPressed { get; set; }
        public bool ShiftKeyPressed { get; set; }
        public bool AltKeyPressed { get; set; }
        public InputKey MainKey { get; set; } = InputKey.None;

        public List<InputKey> InvalidKeys { get; set; } =
        [
            InputKey.Back, InputKey.Space, InputKey.Capital, InputKey.LWin, InputKey.RWin, InputKey.Escape,
            InputKey.Delete, InputKey.Return, InputKey.Enter, InputKey.Pause,
        ];

        public HotKey(InputKey mainKey = InputKey.None, bool ctrlKeyPressed = false, bool shiftKeyPressed = false, bool altKeyPressed = false)
        {
            CtrlKeyPressed = ctrlKeyPressed;
            ShiftKeyPressed = shiftKeyPressed;
            AltKeyPressed = altKeyPressed;

            if (IsControlKey(mainKey))
                CtrlKeyPressed = true;
            else if (IsShiftKey(mainKey))
                ShiftKeyPressed = true;
            else if (IsAltKey(mainKey))
                AltKeyPressed = true;
            else
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

            MainKey = (InputKey)Enum.Parse(typeof(InputKey), text, true);
        }

        public void Clear()
        {
            CtrlKeyPressed = false;
            AltKeyPressed = false;
            ShiftKeyPressed = false;
            MainKey = InputKey.None;
        }

        public override string ToString()
        {
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

            if (MainKey == InputKey.None)
                shortcut = shortcut[..^1];
            else if (InvalidKeys.Contains(MainKey))
                shortcut = MainKey.ToString();
            else
                shortcut += MainKey.ToString();

            return shortcut;
        }

        public override bool Equals(object? obj)
        {
            if (obj is HotKey otherHk)
                return GetHashCode() == otherHk.GetHashCode();

            return false;
        }

        internal static bool IsCommandKey(InputKey keyCode) => (IsControlKey(keyCode) || IsShiftKey(keyCode) || IsAltKey(keyCode));

        internal static bool IsControlKey(InputKey keyCode)
        {
            return (keyCode == InputKey.LControlKey || keyCode == InputKey.RControlKey || 
                    keyCode == InputKey.ControlKey || keyCode == InputKey.Control);
        }

        internal static bool IsShiftKey(InputKey keyCode)
        {
            return (keyCode == InputKey.LShiftKey || keyCode == InputKey.RShiftKey || 
                    keyCode == InputKey.ShiftKey || keyCode == InputKey.Shift);
        }

        internal static bool IsAltKey(InputKey keyCode)
        {
            return (keyCode == InputKey.LMenu || keyCode == InputKey.RMenu || 
                    keyCode == InputKey.Menu || keyCode == InputKey.Alt);
        }

        public override int GetHashCode() => HashCode.Combine(MainKey, CtrlKeyPressed, ShiftKeyPressed, AltKeyPressed);

        public static implicit operator string(HotKey hotKey) => hotKey.ToString();
    }
}
