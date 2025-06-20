namespace InputHookManager.Enums;

public enum InputKey : short
{
    MouseLeft = -100,
    MouseRight = -99,
    MouseMiddle = -98,
    Button1 = -97,
    Button2 = -96,

    None = 0,
    Esc = 1,
    D1 = 2,
    D2 = 3,
    D3 = 4,
    D4 = 5,
    D5 = 6,
    D6 = 7,
    D7 = 8,
    D8 = 9,
    D9 = 10,
    D0 = 11,
    Minus = 12,
    Plus = 13,
    Backspace = 14,
    Tab = 15,
    Q = 16,
    W = 17,
    E = 18,
    R = 19,
    T = 20,
    Y = 21,
    U = 22,
    I = 23,
    O = 24,
    P = 25,
    OpenBracket = 26,
    CloseBracket = 27,
    Enter = 28,
    LControl = 29,
    A = 30,
    S = 31,
    D = 32,
    F = 33,
    G = 34,
    H = 35,
    J = 36,
    K = 37,
    L = 38,
    Semicolon = 39,
    Quote = 40,
    Tilde = 41,
    LShift = 42,
    Backslash = 43,
    Z = 44,
    X = 45,
    C = 46,
    V = 47,
    B = 48,
    N = 49,
    M = 50,
    Comma = 51,
    Period = 52,
    Slash = 53,
    RShift = 54,
    NumMul = 55,
    LAlt = 56,
    Space = 57,
    CapsLock = 58,
    F1 = 59,
    F2 = 60,
    F3 = 61,
    F4 = 62,
    F5 = 63,
    F6 = 64,
    F7 = 65,
    F8 = 66,
    F9 = 67,
    F10 = 68,
    NumLock = 69,
    ScrollLock = 70,
    Num7 = 71,
    Num8 = 72,
    Num9 = 73,
    NumMin = 74,
    Num4 = 75,
    Num5 = 76,
    Num6 = 77,
    NumAdd = 78,
    Num1 = 79,
    Num2 = 80,
    Num3 = 81,
    Num0 = 82,
    NumDel = 83,
    F11 = 87,
    F12 = 88,
    F13 = 100,
    F14 = 101,
    F15 = 102,
    F16 = 103,
    F17 = 104,
    F18 = 105,
    Kana = 112,
    F19 = 113,
    Convert = 121,
    NoConvert = 123,
    Yen = 125,
    NumQqals = 141,
    Circumflex = 144,
    At = 145,
    Colon = 146,
    Underline = 147,
    Kanji = 148,
    Stop = 149,
    Ax = 150,
    Uulabeled = 151,
    //NumEnter = 156,
    //RControl = 157,
    Section = 167,
    Numcomma = 179,
    Divine = 181,
    Sysrq = 183,
    RMenu = 184,
    Function = 196,
    Pause = 197,
    //Home = 199,
    Up = 200,
    Prior = 201,
    Left = 203,
    Right = 205,
    //End = 207,
    Down = 208,
    Next = 209,
    //Insert = 210,
    //Delete = 211,
    Clear = 218,
    LMeta = 219,
    LWwin = 219,
    RMeta = 220,
    RWWin = 220,
    Apps = 221,
    Power = 222,
    Sleep = 223,

    NumEnter = 284,
    RControl = 285,

    NumDiv = 309,

    RAlt = 312,

    ArrowUp = 328,

    LWin = 347,
    RWin = 348,
    ContextMenu = 349,


    Insert = 338,
    Delete = 339,


    Home = 327,
    PageUp = 329,

    ArrowLeft = 331,

    ArrowRight = 333,

    End = 335,
    ArrowDown = 336,
    PageDown = 337,
}

public static class InputKeyExtensions
{
    public static bool IsMouseKey(this InputKey key) => key < InputKey.None;
    public static bool IsKeyboardKey(this InputKey key) => key >= InputKey.None;
    public static bool IsCommandKey(this InputKey key) => key is InputKey.LControl or InputKey.RControl or InputKey.LShift or InputKey.RShift or InputKey.LAlt or InputKey.RAlt;
    public static bool IsModifierKey(this InputKey key) => key is InputKey.LControl or InputKey.RControl or InputKey.LShift or InputKey.RShift or InputKey.LAlt or InputKey.RAlt or InputKey.LWin or InputKey.RWin;
}

public class InputKeyArrayComparer : IEqualityComparer<InputKey[]>
{
    public bool Equals(InputKey[]? x, InputKey[]? y)
    {
        if (x == y) return true;
        if (x == null || y == null) return false;
        if (x.Length != y.Length) return false;
        return x.SequenceEqual(y);
    }

    public int GetHashCode(InputKey[]? obj)
    {
        if (obj == null)
            return 0;

        unchecked // permite overflow de inteiros (sem exception)
        {
            int hash = 17;
            foreach (var item in obj)
                hash = hash * 31 + item.GetHashCode();
            return hash;
        }
    }
}