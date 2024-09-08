using System;
using System.Runtime.InteropServices;

namespace InputHookManager.Utils
{
    public static class WinUtils
    {
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

        public static bool IsActiveWindow(IntPtr windowHandle)
        {
            var activeHandle = GetForegroundWindow();
            return (activeHandle == windowHandle);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();
    }
}