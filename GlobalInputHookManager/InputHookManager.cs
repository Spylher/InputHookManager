using GlobalInputHookManager.Enums;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GlobalInputHookManager
{
    public class InputHookManager : IDisposable
    {
        public static IntPtr Hwnd { get; set; } = IntPtr.Zero;
        public static Dictionary<HotKey, Action<Object>> KeyMappingsPressed = [];
        public static Dictionary<HotKey, Action<Object>> KeyMappingsReleased = [];

        public static Dictionary<Keys, bool> KeyStates = [];
        public static List<HotKey> AllowedKeys = [];
        public static HotKey KeyPressed = new();

        public void Install()
        {
            if (KeyboardHook.Id == IntPtr.Zero)
            {
                KeyboardHook.Id = SetHook(KeyboardHook.Proc);
                //MouseHookId = SetMouseHook(_mouseProc);
                //Enable();
            }

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                KeyStates[key] = false;
        }

        public void Uninstall()
        {
            //Disable();
            UnhookWindowsHookEx(KeyboardHook.Id);
            KeyboardHook.Id = IntPtr.Zero;
            //UnhookWindowsHookEx(MouseHookId);
            //MouseHookId = IntPtr.Zero;
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

        private static IntPtr SetHook(KeyboardHook.LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule!;
            return SetWindowsHookEx((int)WinMessages.WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        public void Dispose()
        {
            ClearActions();
            Uninstall();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHook.LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


    }
}
