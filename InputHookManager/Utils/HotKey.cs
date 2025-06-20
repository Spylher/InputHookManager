using InputHookManager.Enums;
namespace InputHookManager.Utils;

public class HotKey
{
    public bool CtrlKeyPressed { get; set; }
    public bool ShiftKeyPressed { get; set; }
    public bool AltKeyPressed { get; set; }
    public InputKey MainKey { get; set; } = InputKey.None;

    public List<InputKey> InvalidKeys { get; set; } =
    [
        InputKey.Backslash, InputKey.Space, InputKey.CapsLock, InputKey.LWin, InputKey.RWin, InputKey.Esc,
            InputKey.Delete, InputKey.Enter, InputKey.Pause,
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

    public HotKey(InputKey key) : this([key])
    {
    }

    public HotKey(InputKey[] keys)
    {
        CtrlKeyPressed = keys.Contains(InputKey.LControl) || keys.Contains(InputKey.RControl);
        ShiftKeyPressed = keys.Contains(InputKey.LShift) || keys.Contains(InputKey.RShift);
        AltKeyPressed = keys.Contains(InputKey.LAlt) || keys.Contains(InputKey.RAlt);
        MainKey = keys.FirstOrDefault(k => !IsCommandKey(k), InputKey.None);
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

        if (MainKey == InputKey.None && shortcut.Length > 0)
            shortcut = shortcut[..^1];
        else if (MainKey == InputKey.None)
            shortcut = "None";
        else if (InvalidKeys.Contains(MainKey))
            shortcut = MainKey.ToString();
        else
            shortcut += MainKey.ToString();

        return shortcut;
    }

    public InputKey[] ToInputKey()
    {
        var keys = new List<InputKey>();

        if (CtrlKeyPressed)
            keys.Add(InputKey.LControl);
        if (ShiftKeyPressed)
            keys.Add(InputKey.LShift);
        if (AltKeyPressed)
            keys.Add(InputKey.LAlt);
        if (MainKey != InputKey.None)
            keys.Add(MainKey);

        return keys.ToArray();
    }

    public override bool Equals(object? obj)
    {
        if (obj is HotKey otherHk)
            return GetHashCode() == otherHk.GetHashCode();

        if (obj is InputKey[] keys)
        {
            var ctrlDown = keys.Contains(InputKey.LControl) || keys.Contains(InputKey.RControl);
            var shiftDown = keys.Contains(InputKey.LShift) || keys.Contains(InputKey.RShift);
            var altDown = keys.Contains(InputKey.LAlt) || keys.Contains(InputKey.RAlt);

            if (CtrlKeyPressed != ctrlDown || ShiftKeyPressed != shiftDown || AltKeyPressed != altDown)
                return false;

            return keys.Contains(MainKey);
        }

        if (obj is InputKey key)
        {
            return MainKey == key;
        }

        return false;
    }



    internal static bool IsCommandKey(InputKey keyCode) => (IsControlKey(keyCode) || IsShiftKey(keyCode) || IsAltKey(keyCode));

    internal static bool IsControlKey(InputKey keyCode) => keyCode is InputKey.LControl or InputKey.RControl;

    internal static bool IsShiftKey(InputKey keyCode) => keyCode is InputKey.LShift or InputKey.RShift;

    internal static bool IsAltKey(InputKey keyCode) => keyCode is InputKey.LAlt or InputKey.RAlt;

    public override int GetHashCode() => HashCode.Combine(MainKey, CtrlKeyPressed, ShiftKeyPressed, AltKeyPressed);

    public static implicit operator string(HotKey hotKey) => hotKey.ToString();
}
