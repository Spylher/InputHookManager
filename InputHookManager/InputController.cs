using InputHookManager.Enums;
using InputHookManager.Utils;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InputHookManager
{
    public partial class InputController : IDisposable
    {
        private IntPtr Hwnd { get; set; } = IntPtr.Zero;
        private Dictionary<HotKey, Action<Object>> KeyMappingsPressed = [];
        private Dictionary<HotKey, Action<Object>> KeyMappingsReleased = [];
        private Dictionary<InputKey, bool> KeyState = [];
        private List<HotKey> AllowedKeys = [];
        private List<HotKey> SuppressedKeys = [];
        private HotKey KeyPressed = new();
        private bool IsHookActive;

        public InputController(bool runInSeparateThread = true)
        {
            if (runInSeparateThread)
                Task.Run(() =>
                {
                    InitializeHooks();
                    CaptureMessages();
                });
            else
                InitializeHooks();
        }

        private void InitializeHooks()
        {
            //keyboard
            KeyboardProc = KeyboardHookCallback;
            KeyboardId = SetKeyboardHook(KeyboardProc);

            //mouse
            MouseProc = MouseHookCallback;
            MouseHookId = SetMouseHook(MouseProc);

            Enable();
        }

        public void Enable()
        {
            IsHookActive = true;

            foreach (InputKey key in Enum.GetValues(typeof(InputKey)))
                KeyState[key] = false;
        }

        public void Disable() => IsHookActive = false;

        public void CaptureMessages()
        {
            while (GetMessage(out WinUtils.MESSAGE _, IntPtr.Zero, 0, 0))
            {
                Thread.Sleep(1);
            }
        }

        public bool Attach(int pid) => Attach(Process.GetProcessById(pid));

        public bool Attach(Process process)
        {
            if (process.MainWindowHandle != 0)
            {
                Hwnd = process.MainWindowHandle;
                return true;
            }

            return false;
        }

        public bool Attach(IntPtr hwnd)
        {
            if (hwnd == 0)
                return false;
            
            Hwnd = hwnd;
            return true;
        }

        public void RegisterAction(HotKey hotkey, Action<object> act, bool suppressDefault = false, ActionMode actionMode = ActionMode.Windowed, KeyState keyState = Enums.KeyState.Pressed)
        {
            if (keyState == Enums.KeyState.Released)
            {
                UnregisterAction(hotkey, Enums.KeyState.Released);
                KeyMappingsReleased.Add(hotkey, act);
            }
            else
            {
                UnregisterAction(hotkey);
                KeyMappingsPressed.Add(hotkey, act);
            }

            if (actionMode == ActionMode.Global)
                AllowedKeys.Add(hotkey);
            if (suppressDefault)
                SuppressedKeys.Add(hotkey);
        }

        public void UnregisterAction(HotKey hotkey, KeyState keyState = Enums.KeyState.Pressed)
        {
            if (AllowedKeys.Contains(hotkey))
                AllowedKeys.Remove(hotkey);

            if (SuppressedKeys.Contains(hotkey))
                SuppressedKeys.Remove(hotkey);

            if (keyState == Enums.KeyState.Released)
            {
                if (KeyMappingsReleased.ContainsKey(hotkey))
                    KeyMappingsReleased.Remove(hotkey);
            }
            else
            {
                if (KeyMappingsPressed.ContainsKey(hotkey))
                    KeyMappingsPressed.Remove(hotkey);
            }
        }

        public void ClearActions()
        {
            AllowedKeys.Clear();
            SuppressedKeys.Clear();
            KeyMappingsPressed.Clear();
            KeyMappingsReleased.Clear();
        }

        public bool IsKeyDown(InputKey key) => KeyState[key];

        private IntPtr SetKeyboardHook(LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule!;
            return SetWindowsHookEx((int)WinMessages.WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private IntPtr SetMouseHook(LowLevelMouseProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule!;
            return SetWindowsHookEx((int)WinMessages.WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        public void Dispose()
        {
            Disable();
            ClearActions();
            UnhookWindowsHookEx(KeyboardId);
            UnhookWindowsHookEx(MouseHookId);
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern bool GetMessage(out WinUtils.MESSAGE lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    }

}
