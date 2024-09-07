using MySpy.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GlobalInputHookManager.Utils;
using System.Runtime.InteropServices;

namespace GlobalInputHookManager
{
    using static WinMessages;
    using static WindowUtils;

    public class InputHookManager
    {
        //public static IntPtr HWND { get; set; } = IntPtr.Zero;

        //public static Dictionary<HotKey, Action<Object>> KeyMappingsPressed = new Dictionary<HotKey, Action<Object>>();
        //public static Dictionary<HotKey, Action<Object>> KeyMappingsReleased = new Dictionary<HotKey, Action<Object>>();
        //public static HotKey KeyStart = new HotKey();
        //public static List<HotKey> AllowedKeys { get; set; } = new List<HotKey>();
        //public static HotKey KeyPressed = new HotKey();

        //public void Install()
        //{
        //    if (KeyboardHookId == IntPtr.Zero)
        //    {
        //        KeyboardHookId = SetHook(_proc);
        //        MouseHookId = SetMouseHook(_mouseProc);
        //        Enable();
        //    }

        //    foreach (Keys key in Enum.GetValues(typeof(Keys)))
        //        KeyStates[key] = false;
        //}

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        //[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
