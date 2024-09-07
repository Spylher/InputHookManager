using MySpy.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GlobalInputHookManager.Utils;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GlobalInputHookManager
{
    using static WinMessages;
    //using static WindowUtils;
    using static KeyboardHook;

    public class InputHookManager
    {
        public static IntPtr HWND { get; set; } = IntPtr.Zero;
        public static Dictionary<HotKey, Action<Object>> KeyMappingsPressed = new Dictionary<HotKey, Action<Object>>();
        public static Dictionary<HotKey, Action<Object>> KeyMappingsReleased = new Dictionary<HotKey, Action<Object>>();

        public static Dictionary<Keys, bool> KeyStates = new Dictionary<Keys, bool>();
        public static List<HotKey> AllowedKeys = new List<HotKey>();
        public static HotKey KeyPressed = new HotKey();

        public void Install()
        {
            if (Id == IntPtr.Zero)
            {
                KeyboardHook.Id = SetHook(KeyboardProc);
                //MouseHookId = SetMouseHook(_mouseProc);
                //Enable();
            }

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                KeyStates[key] = false;
        }

        public void Uninstall()
        {
            //Disable();
            UnhookWindowsHookEx(Id);
            Id = IntPtr.Zero;
            //UnhookWindowsHookEx(MouseHookId);
            //MouseHookId = IntPtr.Zero;
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx((int)WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public void RegisterAction(HotKey hotkey, Action<Object> act, KeyState keyState = KeyState.Released)
        {
            UnregisterAction(hotkey, KeyState.Pressed);
            UnregisterAction(hotkey);

            if (keyState == KeyState.Pressed)
                KeyMappingsPressed.Add(hotkey, act);
            else if (keyState == KeyState.Released)
                KeyMappingsReleased.Add(hotkey, act);
        }

        public void UnregisterAction(HotKey hotkey, KeyState keyState = KeyState.Released)
        {

            if (keyState == KeyState.Pressed)
            {
                foreach (var keyValuePair in KeyMappingsPressed)
                    if (keyValuePair.Key.Equals(hotkey))
                        KeyMappingsPressed.Remove(hotkey);

            }
            else if (keyState == KeyState.Released)
            {
                foreach (var keyValuePair in KeyMappingsReleased)
                    if (keyValuePair.Key.Equals(hotkey))
                        KeyMappingsReleased.Remove(hotkey);
            }
        }

        public void ClearActions()
        {
            KeyMappingsPressed.Clear();
            KeyMappingsReleased.Clear();
        }
        public bool IsKeyDown(Keys key)
        {
            return KeyStates[key];
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
