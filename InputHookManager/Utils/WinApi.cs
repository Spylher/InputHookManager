using System.Runtime.InteropServices;
using InputHookManager.Enums;

namespace InputHookManager.Utils
{
    using static WinMessages;
    internal class WinApi : ISimulator
    {
        public static bool IsActiveWindow(IntPtr windowHandle)
        {
            var activeHandle = GetForegroundWindow();
            return (activeHandle == windowHandle);
        }

        #region Mouse Events
        public void SendClick()
        {
            var mouseDown = new INPUT[1];
            mouseDown[0].type = (int)INPUT_MOUSE;
            mouseDown[0].U.mi.dwFlags = (int)MOUSEEVENTF_LEFTDOWN;
            SendInput((uint)mouseDown.Length, mouseDown, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(10);

            var mouseUp = new INPUT[1];
            mouseUp[0].type = (int)INPUT_MOUSE;
            mouseUp[0].U.mi.dwFlags = (int)MOUSEEVENTF_LEFTUP;
            SendInput((uint)mouseUp.Length, mouseUp, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        public void SendClick(int x, int y, bool relative = false)
        {
            MoveMouse(x, y, relative);

            var mouseDown = new INPUT[1];
            mouseDown[0].type = (int)INPUT_MOUSE;
            mouseDown[0].U.mi.dwFlags = (int)MOUSEEVENTF_LEFTDOWN;
            SendInput((uint)mouseDown.Length, mouseDown, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(10);

            var mouseUp = new INPUT[1];
            mouseUp[0].type = (int)INPUT_MOUSE;
            mouseUp[0].U.mi.dwFlags = (int)MOUSEEVENTF_LEFTUP;
            SendInput((uint)mouseUp.Length, mouseUp, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        public void SendRightClick()
        {
            var mouseDown = new INPUT[1];
            mouseDown[0].type = (int)INPUT_MOUSE;
            mouseDown[0].U.mi.dwFlags = (int)MOUSEEVENTF_RIGHTDOWN;
            SendInput((uint)mouseDown.Length, mouseDown, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(10);

            var mouseUp = new INPUT[1];
            mouseUp[0].type = (int)INPUT_MOUSE;
            mouseUp[0].U.mi.dwFlags = (int)MOUSEEVENTF_RIGHTUP;
            SendInput((uint)mouseUp.Length, mouseUp, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        public void SendRightClick(int x, int y, bool relative = false)
        {
            MoveMouse(x, y, relative);

            var mouseDown = new INPUT[1];
            mouseDown[0].type = (int)INPUT_MOUSE;
            mouseDown[0].U.mi.dwFlags = (int)MOUSEEVENTF_RIGHTDOWN;
            SendInput((uint)mouseDown.Length, mouseDown, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(10);

            var mouseUp = new INPUT[1];
            mouseUp[0].type = (int)INPUT_MOUSE;
            mouseUp[0].U.mi.dwFlags = (int)MOUSEEVENTF_RIGHTUP;
            SendInput((uint)mouseUp.Length, mouseUp, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        public void SendMiddleButton()
        {
            var mouseDown = new INPUT[1];
            mouseDown[0].type = (int)INPUT_MOUSE;
            mouseDown[0].U.mi.dwFlags = (int)MOUSEEVENTF_MIDDLEDOWN;
            SendInput((uint)mouseDown.Length, mouseDown, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(10);

            var mouseUp = new INPUT[1];
            mouseUp[0].type = (int)INPUT_MOUSE;
            mouseUp[0].U.mi.dwFlags = (int)MOUSEEVENTF_MIDDLEUP;
            SendInput((uint)mouseUp.Length, mouseUp, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        public void SendXButton1()
        {
            var mouseDown = new INPUT[1];
            mouseDown[0].type = (int)INPUT_MOUSE;
            mouseDown[0].U.mi.dwFlags = (int)MOUSEEVENTF_XDOWN;
            mouseDown[0].U.mi.mouseData = 0x0001; // XBUTTON1
            SendInput((uint)mouseDown.Length, mouseDown, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(10);

            var mouseUp = new INPUT[1];
            mouseUp[0].type = (int)INPUT_MOUSE;
            mouseUp[0].U.mi.dwFlags = (int)MOUSEEVENTF_XUP;
            mouseUp[0].U.mi.mouseData = 0x0001; // XBUTTON1
            SendInput((uint)mouseUp.Length, mouseUp, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        public void SendXButton2()
        {
            var mouseDown = new INPUT[1];
            mouseDown[0].type = (int)INPUT_MOUSE;
            mouseDown[0].U.mi.dwFlags = (int)MOUSEEVENTF_XDOWN;
            mouseDown[0].U.mi.mouseData = 0x0002; // XBUTTON2
            SendInput((uint)mouseDown.Length, mouseDown, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(10);

            var mouseUp = new INPUT[1];
            mouseUp[0].type = (int)INPUT_MOUSE;
            mouseUp[0].U.mi.dwFlags = (int)MOUSEEVENTF_XUP;
            mouseUp[0].U.mi.mouseData = 0x0002; // XBUTTON2
            SendInput((uint)mouseUp.Length, mouseUp, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }


        public void MoveMouse(int x, int y, bool relative)
        {
            if (!relative)
            {
                SetMouse(x, y);
                return;
            }

            var mouseMove = new INPUT[1];
            mouseMove[0].type = (int)INPUT_MOUSE;
            mouseMove[0].U.mi.dx = x;
            mouseMove[0].U.mi.dy = y;
            mouseMove[0].U.mi.dwFlags = (uint)MOUSEEVENTF_MOVE;
            SendInput((uint)mouseMove.Length, mouseMove, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        public void SetMouseWheel(int delta)
        {
            var mouseWheel = new INPUT[1];
            mouseWheel[0].type = (int)INPUT_MOUSE;
            mouseWheel[0].U.mi.mouseData = (uint)delta;
            mouseWheel[0].U.mi.dwFlags = (uint)MOUSEEVENTF_WHEEL;
            SendInput((uint)mouseWheel.Length, mouseWheel, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        public void SetMouseHWheel(int delta)
        {
            var mouseWheel = new INPUT[1];
            mouseWheel[0].type = (int)INPUT_MOUSE;
            mouseWheel[0].U.mi.mouseData = (uint)delta;
            mouseWheel[0].U.mi.dwFlags = (uint)MOUSEEVENTF_HWHEEL;
            SendInput((uint)mouseWheel.Length, mouseWheel, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        private void SetMouse(int x, int y)
        {
            var mouseMove = new INPUT[1];
            mouseMove[0].type = (int)INPUT_MOUSE;
            mouseMove[0].U.mi.dx = x;
            mouseMove[0].U.mi.dy = y;
            mouseMove[0].U.mi.dwFlags = (uint)MOUSEEVENTF_MOVE | (uint)MOUSEEVENTF_ABSOLUTE;
            SendInput((uint)mouseMove.Length, mouseMove, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        private void MouseUp(InputKey key)
        {
            var mouseUp = new INPUT[1];
            mouseUp[0].type = (int)INPUT_MOUSE;
            mouseUp[0].U.mi.dwFlags = (uint)MOUSEEVENTF_LEFTUP;

            if (key == InputKey.MouseRight)
                mouseUp[0].U.mi.dwFlags = (uint)MOUSEEVENTF_RIGHTUP;
            else if (key == InputKey.MouseMiddle)
                mouseUp[0].U.mi.dwFlags = (uint)MOUSEEVENTF_MIDDLEUP;
            else if (key == InputKey.Button1)
            {
                mouseUp[0].U.mi.dwFlags = (uint)MOUSEEVENTF_XUP;
                mouseUp[0].U.mi.mouseData = 0x0001;
            }
            else if (key == InputKey.Button2)
            {
                mouseUp[0].U.mi.dwFlags = (uint)MOUSEEVENTF_XUP;
                mouseUp[0].U.mi.mouseData = 0x0002;
            }

            SendInput((uint)mouseUp.Length, mouseUp, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        private void MouseDown(InputKey key)
        {
            var mouseUp = new INPUT[1];
            mouseUp[0].type = (int)INPUT_MOUSE;
            mouseUp[0].U.mi.dwFlags = (uint)MOUSEEVENTF_LEFTDOWN;

            if (key == InputKey.MouseRight)
                mouseUp[0].U.mi.dwFlags = (uint)MOUSEEVENTF_RIGHTDOWN;
            else if (key == InputKey.MouseMiddle)
                mouseUp[0].U.mi.dwFlags = (uint)MOUSEEVENTF_MIDDLEDOWN;
            else if (key == InputKey.Button1)
            {
                mouseUp[0].U.mi.dwFlags = (uint)MOUSEEVENTF_XDOWN;
                mouseUp[0].U.mi.mouseData = 0x0001;
            }
            else if (key == InputKey.Button2)
            {
                mouseUp[0].U.mi.dwFlags = (uint)MOUSEEVENTF_XDOWN;
                mouseUp[0].U.mi.mouseData = 0x0002;
            }

            SendInput((uint)mouseUp.Length, mouseUp, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        #endregion

        #region Keyboard Events

        public void KeyDown(InputKey key)
        {
            if (key < InputKey.None)
            {
                MouseDown(key);
                return;
            }

            var keyDown = new INPUT[1];
            keyDown[0].type = (int)INPUT_KEYBOARD;
            keyDown[0].U.ki.wVk = 0;
            keyDown[0].U.ki.wScan = (ushort)key;
            keyDown[0].U.ki.dwFlags = (uint)KEYEVENTF_SCANCODE;
            SendInput((uint)keyDown.Length, keyDown, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        public void KeyDown(params InputKey[] keys)
        {
            foreach (var inputKey in keys)
                KeyDown(inputKey);
        }

        public void KeyUp(InputKey key)
        {
            if (key < InputKey.None)
            {
                MouseUp(key);
                return;
            }

            var keyUp = new INPUT[1];
            keyUp[0].type = (int)INPUT_KEYBOARD;
            keyUp[0].U.ki.wVk = 0;
            keyUp[0].U.ki.wScan = (ushort)key;
            keyUp[0].U.ki.dwFlags = (uint)KEYEVENTF_KEYUP | (uint)KEYEVENTF_SCANCODE;
            SendInput((uint)keyUp.Length, keyUp, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(1);
        }

        public void KeyUp(params InputKey[] keys)
        {
            foreach (var inputKey in keys)
                KeyUp(inputKey);
        }

        public void SendKey(InputKey key)
        {
            KeyDown(key);
            KeyUp(key);
        }

        public void SendKey(params InputKey[] keys)
        {
            var keyboardKeys = keys.Where(k => k >= InputKey.None).ToArray();
            var mouseKeys = keys.Where(k => k < InputKey.None).ToArray();

            KeyDown(keyboardKeys);
            KeyDown(mouseKeys);
            KeyUp(mouseKeys);
            KeyUp(keyboardKeys);
        }

        #endregion

        #region  Imports
        [StructLayout(LayoutKind.Sequential)]
        public struct MESSAGE
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;      // Vk code
            public uint scanCode;    // Scan code
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public struct POINT
        {
            public int X;
            public int Y;
            public static bool Relative;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public uint type;
            public InputUnion U;
        }


        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;

            [FieldOffset(0)]
            public KEYBDINPUT ki;

            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();



        #endregion

    }
}