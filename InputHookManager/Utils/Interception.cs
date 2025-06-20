using System.Runtime.InteropServices;
using InputHookManager.Enums;
using InputHookManager.Models;
namespace InputHookManager.Utils;

internal static unsafe class Interception
{
    public static Action<int, int>? OnMouseMove;
    public static Action<int>? OnMouseWheel;
    public static Action<InputKey>? OnKeyDown;
    public static Action<InputKey>? OnKeyIsPress;
    public static Action<InputKey>? OnKeyUp;
    public static Func<int, int, bool>? CancelableOnMouseMove;
    public static Func<int, bool>? CancelableOnMouseWheel;
    public static Func<InputKey, bool>? CancelableOnKeyDown;
    public static Func<InputKey, bool>? CancelableOnKeyIsPress;
    public static Func<InputKey, bool>? CancelableOnKeyUp;
    public static KeyboardDriver* KeyboardDriver;
    public static MouseDriver* MouseDriver;
    public static Context Context;
    static long* keyStates = (long*)Marshal.AllocCoTaskMem(0x80) + 0x08;
    public static Thread KeyboardThread;
    public static Thread MouseThread;

    static Interception()
    {
        //if (Debugger.IsAttached)
        //    return;

        Context = Context.Create();
        Context.SetFilter(Filter.All);

        KeyboardThread = new Thread(KeyboardUpdater) { Priority = ThreadPriority.Highest, IsBackground = true };
        KeyboardThread.Start();

        MouseThread = new Thread(MouseUpdater) { Priority = ThreadPriority.Highest, IsBackground = true };
        MouseThread.Start();
    }

    static void MarkKeyIsDown(InputKey key) => keyStates[(int)key / 64] |= 1L << ((int)key % 64);
    static void MarkKeyIsUp(InputKey key) => keyStates[(int)key / 64] &= ~(1L << ((int)key % 64));
    public static bool IsKeyDown(InputKey key) => (keyStates[(int)key / 64] & (1L << ((int)key % 64))) != 0;
    public static bool IsKeyUp(InputKey key) => (keyStates[(int)key / 64] & (1L << ((int)key % 64))) == 0;

    static void KeyboardUpdater()
    {
        KeyStroke stroke;

        while ((KeyboardDriver = Context.WaitKeyboardInput()) != null)
            try
            {
                while (KeyboardDriver->Receive(&stroke))
                {
                    var key = (InputKey)((stroke.State & KeyState.E0) != 0 ? stroke.Code + 0x100 : stroke.Code);
                    if ((stroke.State & KeyState.Up) == 0
                        ? IsKeyUp(key)
                          ? InternalOnKeyDown(key)
                          : InternalOnKeyIsPress(key)
                        : InternalOnKeyUp(key))
                        KeyboardDriver->Send(&stroke);
                }
            }
            catch { KeyboardDriver->Send(&stroke); }
    }

    static void MouseUpdater()
    {
        while (true)
        {
            MouseDriver = Context.WaitMouseInput();

            if (MouseDriver == null)
                continue;

            while (true)
            {
                MouseStroke stroke;
                try
                {
                    while (MouseDriver->Receive(&stroke))
                    {
                        if (stroke.X != 0 || stroke.Y != 0)
                            if (!InternalOnMouseMove(stroke.X, stroke.Y))
                                *(long*)&stroke.X = 0;

                        if (stroke.State != default)
                        {
                            if ((stroke.State & MouseState.Wheel) != 0)
                                if (!InternalOnMouseWheel(stroke.Rolling))
                                    stroke.State &= ~MouseState.Wheel;

                            for (var keyIndex = 0; keyIndex < 5; keyIndex++)
                            for (var stateIndex = 0; stateIndex < 2; stateIndex++)
                            {
                                var mask = 1 << keyIndex * 2 + stateIndex;
                                if (((int)stroke.State & mask) != 0)
                                    if (stateIndex == 0
                                            ? !InternalOnKeyDown((InputKey)(-100 + keyIndex))
                                            : !InternalOnKeyUp((InputKey)(-100 + keyIndex)))
                                        stroke.State = (MouseState)((int)stroke.State & ~mask);
                            }
                        }

                        MouseDriver->Send(&stroke);
                    }
                }
                catch
                {
                    MouseDriver->Send(&stroke);
                }
            }
        }
    }

    static void ToKeyStroke(KeyStroke* stroke, InputKey key, bool down)
    {
        if (!down)
            stroke->State = KeyState.Up;

        var code = (short)key;
        if (code >= 0x100)
        {
            code -= 0x100;
            stroke->State |= KeyState.E0;
        }
        else if (code < 0)
        {
            code += 100;
            stroke->State |= KeyState.E0;
        }
        stroke->Code = (ushort)code;
    }

    static bool InternalOnMouseMove(int x, int y)
    {
        OnMouseMove?.Invoke(x, y);
        if (CancelableOnMouseMove is not null)
            return CancelableOnMouseMove(x, y);
        return true;
    }

    static bool InternalOnMouseWheel(int rolling)
    {
        OnMouseWheel?.Invoke(rolling);
        if (CancelableOnMouseWheel is not null)
            return CancelableOnMouseWheel(rolling);
        return true;
    }

    public static bool InternalOnKeyDown(InputKey key)
    {
        OnKeyDown?.Invoke(key);
        if (CancelableOnKeyDown is not null)
            if (!CancelableOnKeyDown(key))
                return false;

        if (InternalOnKeyIsPress(key))
        {
            MarkKeyIsDown(key);
            return true;
        }
        return false;
    }

    static bool InternalOnKeyUp(InputKey key)
    {
        OnKeyUp?.Invoke(key);
        if (CancelableOnKeyUp is not null)
        {
            if (CancelableOnKeyUp(key))
            {
                MarkKeyIsUp(key);
                return true;
            }
            return false;
        }

        MarkKeyIsUp(key);
        return true;
    }

    static bool InternalOnKeyIsPress(InputKey key)
    {
        OnKeyIsPress?.Invoke(key);
        if (CancelableOnKeyIsPress is not null)
            return CancelableOnKeyIsPress(key);
        return true;
    }

    public static void KeyUp(params InputKey[] keys)
    {
        foreach (var key in keys)
            KeyUp(key);
    }

    public static void KeyUp(InputKey key)
    {
        if (key < InputKey.None)
        {
            if (MouseDriver is null)
                return;

            var stroke = new MouseStroke();
            for (var bitshift = 0; bitshift < 5; bitshift++)
                if (key == (InputKey)((int)InputKey.MouseLeft + bitshift))
                {
                    stroke.State = (MouseState)(1 << bitshift * 2);
                    break;
                }

            MarkKeyIsUp(key);
            MouseDriver->Send(&stroke);
        }
        else
        {
            if (KeyboardDriver is null)
                return;

            var stroke = new KeyStroke();
            ToKeyStroke(&stroke, key, false);
            MarkKeyIsUp(key);
            KeyboardDriver->Send(&stroke);
        }
    }

    public static void KeyDown(params InputKey[] keys)
    {
        foreach (var key in keys)
            KeyDown(key);
    }

    public static void KeyDown(InputKey key)
    {
        if (key < InputKey.None)
        {
            if (MouseDriver is null)
                return;

            var stroke = new MouseStroke();
            for (var bitshift = 0; bitshift < 5; bitshift++)
                if (key == (InputKey)((int)InputKey.MouseLeft + bitshift))
                {
                    stroke.State = (MouseState)(1 << bitshift * 2);
                    break;
                }

            MarkKeyIsDown(key);
            MouseDriver->Send(&stroke);
        }
        else
        {
            if (KeyboardDriver is null)
                return;

            var stroke = new KeyStroke();
            ToKeyStroke(&stroke, key, true);
            MarkKeyIsDown(key);
            KeyboardDriver->Send(&stroke);
        }
    }

    internal static void MouseHandler(InputKey key)
    {
        if (key == InputKey.MouseLeft)
            LeftClick();
        else if (key == InputKey.MouseRight)
            RightClick();
        else if (key == InputKey.MouseMiddle)
            MiddleButton();
        else if (key == InputKey.Button1)
            XButton1();
        else if (key == InputKey.Button2)
            XButton2();
    }

    public static void XButton1()
    {
        if (MouseDriver is null)
            return;

        var downStroke = new MouseStroke { State = MouseState.Button4Down };
        MouseDriver->Send(&downStroke);

        Thread.Sleep(10);

        var upStroke = new MouseStroke { State = MouseState.Button4Up };
        MouseDriver->Send(&upStroke);
    }

    public static void XButton2()
    {
        if (MouseDriver is null)
            return;

        var downStroke = new MouseStroke { State = MouseState.Button5Down };
        MouseDriver->Send(&downStroke);

        Thread.Sleep(10);

        var upStroke = new MouseStroke { State = MouseState.Button5Up };
        MouseDriver->Send(&upStroke);
    }

    public static void MiddleButton()
    {
        if (MouseDriver is null)
            return;

        var downStroke = new MouseStroke { State = MouseState.MiddleButtonDown };
        MouseDriver->Send(&downStroke);

        Thread.Sleep(10);

        var upStroke = new MouseStroke { State = MouseState.MiddleButtonUp };
        MouseDriver->Send(&upStroke);
    }

    public static void SetMouseWheel(short rolling)
    {
        if (MouseDriver is null)
            return;

        var stroke = new MouseStroke { State = MouseState.Wheel, Rolling = rolling };
        MouseDriver->Send(&stroke);
    }

    public static void SetMouseHWheel(short rolling)
    {
        if (MouseDriver is null)
            return;

        var stroke = new MouseStroke { State = MouseState.HWheel, Rolling = rolling };
        MouseDriver->Send(&stroke);
    }

    public static void MoveMouse(int x, int y)
    {
        if (MouseDriver is null)
            return;

        var stroke = new MouseStroke { X = x, Y = y, Flags = MouseFlag.MoveRelative };
        MouseDriver->Send(&stroke);
    }

    public static void SetMouse(int x, int y)
    {
        if (MouseDriver is null)
            return;

        // Prevents the mouse from being set to (0,0) which can cause issues
        if (x == 0 && y == 0)
        {
            x = 1;
            y = 1;
        }

        var stroke = new MouseStroke { X = x, Y = y, Flags = MouseFlag.MoveAbsolute };
        MouseDriver->Send(&stroke);
    }

    internal static void LeftClick()
    {
        if (MouseDriver is null)
            return;

        var downStroke = new MouseStroke { State = MouseState.LeftButtonDown };
        MouseDriver->Send(&downStroke);

        Thread.Sleep(10);

        var upStroke = new MouseStroke { State = MouseState.LeftButtonUp };
        MouseDriver->Send(&upStroke);
    }

    internal static void LeftClick(int x, int y, bool relative = true)
    {
        if (relative)
            MoveMouse(x, y);
        else
            SetMouse(x, y);

        Thread.Sleep(10);

        var downStroke = new MouseStroke { State = MouseState.LeftButtonDown };
        MouseDriver->Send(&downStroke);

        Thread.Sleep(10);

        var upStroke = new MouseStroke { State = MouseState.LeftButtonUp };
        MouseDriver->Send(&upStroke);
    }

    internal static void RightClick()
    {
        if (MouseDriver is null)
            return;

        var downStroke = new MouseStroke { State = MouseState.RightButtonDown };
        MouseDriver->Send(&downStroke);

        Thread.Sleep(10);

        var upStroke = new MouseStroke { State = MouseState.RightButtonUp };
        MouseDriver->Send(&upStroke);
    }

    internal static void RightClick(int x, int y, bool relative = true)
    {
        if (MouseDriver is null)
            return;

        if (relative)
            MoveMouse(x, y);
        else
            SetMouse(x, y);

        Thread.Sleep(10);

        var downStroke = new MouseStroke { State = MouseState.RightButtonDown };
        MouseDriver->Send(&downStroke);

        Thread.Sleep(10);

        var upStroke = new MouseStroke { State = MouseState.RightButtonUp };
        MouseDriver->Send(&upStroke);
    }

    internal static void MouseDown(InputKey key)
    {
        if (MouseDriver is null)
            return;

        var state = MouseState.LeftButtonDown;

        if (key == InputKey.MouseRight)
            state = MouseState.RightButtonDown;
        else if (key == InputKey.MouseMiddle)
            state = MouseState.MiddleButtonDown;
        else if (key == InputKey.Button1)
            state = MouseState.Button4Down;
        else if (key == InputKey.Button2)
            state = MouseState.Button5Down;

        var downStroke = new MouseStroke { State = state };
        MouseDriver->Send(&downStroke);
    }

    internal static void MouseUp(InputKey key)
    {
        if (MouseDriver is null)
            return;

        var state = MouseState.LeftButtonUp;

        if (key == InputKey.MouseRight)
            state = MouseState.RightButtonUp;
        else if (key == InputKey.MouseMiddle)
            state = MouseState.MiddleButtonUp;
        else if (key == InputKey.Button1)
            state = MouseState.Button4Up;
        else if (key == InputKey.Button2)
            state = MouseState.Button5Up;

        var downStroke = new MouseStroke { State = state };
        MouseDriver->Send(&downStroke);
    }
}