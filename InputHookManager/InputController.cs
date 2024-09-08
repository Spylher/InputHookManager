using InputHookManager.Enums;
using InputHookManager.Utils;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InputHookManager
{
    public partial class InputController : IDisposable
    {
        public IntPtr Hwnd { get; set; } = IntPtr.Zero;
        public Dictionary<HotKey, Action<Object>> KeyMappingsPressed = [];
        public Dictionary<HotKey, Action<Object>> KeyMappingsReleased = [];
        public Dictionary<KeyInput, bool> KeyStates = [];

        public List<HotKey> AllowedKeys = [];
        public HotKey KeyPressed = new();

        public InputController()
        {
            //keyboard
            KeyboardProc = KeyboardHookCallback;
            KeyboardId = SetHook(KeyboardProc);

            //mouse
            //KeyboardProc = KeyboardHookCallback;
            //MouseHookId = SetMouseHook(_mouseProc);

            //Enable();
            foreach (KeyInput key in Enum.GetValues(typeof(KeyInput)))
                KeyStates[key] = false;
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

        public bool IsKeyDown(KeyInput key) => KeyStates[key];

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule!;
            return SetWindowsHookEx((int)WinMessages.WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        public void Dispose()
        {
            ClearActions();
            //Disable();
            UnhookWindowsHookEx(KeyboardId);
            //UnhookWindowsHookEx(MouseHookId);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    }

}
