using InputHookManager.Enums;
using InputHookManager.Utils;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InputHookManager
{
    public partial class InputController : IDisposable
    {
        internal IntPtr Hwnd { get; set; } = IntPtr.Zero;
        internal Dictionary<HotKey, Action<Object>> KeyMappingsPressed = [];
        internal Dictionary<HotKey, Action<Object>> KeyMappingsReleased = [];
        internal Dictionary<InputKey, bool> KeysState = [];
        internal List<HotKey> AllowedKeys = [];
        internal List<HotKey> SuppressedKeys = [];
        internal HotKey KeyPressed = new();
        internal bool IsHookActive;

        /// <summary>
        ///     Initialize a <see cref="InputController"/>.
        /// </summary>
        /// <param name="runInSeparateThread"> If <see langword="false"/>, this operation will not run in separate thread</param>.
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

        /// <summary>
        ///     Enable the hook.
        /// </summary>
        public void Enable()
        {
            IsHookActive = true;

            foreach (InputKey key in Enum.GetValues(typeof(InputKey)))
                KeysState[key] = false;
        }

        /// <summary>
        ///     Disable the hook.
        /// </summary>
        public void Disable() => IsHookActive = false;

        private void CaptureMessages()
        {
            while (GetMessage(out WinUtils.MESSAGE _, IntPtr.Zero, 0, 0))
            {
                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///    Attach a main window handle by process ID.
        /// </summary>
        /// <param name="pid"> Process ID </param >.
        public bool Attach(int pid) => Attach(Process.GetProcessById(pid));

        /// <summary>
        ///    Attach a main window handle using a <see cref="Process"/>
        /// </summary>
        /// <param name="process"> Entity Process </param >
        public bool Attach(Process process)
        {
            if (process.MainWindowHandle != 0)
            {
                Hwnd = process.MainWindowHandle;
                return true;
            }

            return false;
        }

        /// <summary>
        ///   Attach a main window handle by ID.
        /// </summary>
        /// <param name="hwnd"> Main window handle ID </param >.
        public bool Attach(IntPtr hwnd)
        {
            if (hwnd == 0)
                return false;

            Hwnd = hwnd;
            return true;
        }

        /// <summary>
        ///     Register the action for the shortcut using a <see cref="HotKey"/>.
        /// </summary>
        /// <param name="hotkey"> Shortcut entity </param>.
        /// <param name="act"> Action to associate a shortcut </param>.
        /// <param name="suppressDefault"> if <see langword="true"/>, the default action will be suppressed </param>.
        /// <param name="actionMode">
        ///     Specifies action mode of shortcut
        ///     Use <see cref="ActionMode.Global"/> for the action work on a global scope,
        ///     or <see cref="ActionMode.Windowed"/> to windowed.
        /// </param>.
        /// <param name="keyState">
        ///     Specifies key state to start the action
        ///     Use <see cref="KeyState.Pressed"/> for the start action while key is pressed,
        ///     or <see cref="KeyState.Released"/> to the start action when key is released,
        /// </param>.
        public void RegisterAction(HotKey hotkey, Action<object> act, bool suppressDefault = false, ActionMode actionMode = ActionMode.Windowed, KeyState keyState = KeyState.Pressed)
        {
            if (keyState == KeyState.Released)
            {
                UnregisterAction(hotkey, KeyState.Released);
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

        /// <summary>
        ///     Unregister the action for the shortcut using a <see cref="HotKey"/>.
        /// </summary>
        /// <param name="hotkey"> Shortcut entity </param>.
        /// <param name="keyState"> Specifies key state to remove of the action </param>.
        public void UnregisterAction(HotKey hotkey, KeyState keyState = KeyState.Pressed)
        {
            if (AllowedKeys.Contains(hotkey))
                AllowedKeys.Remove(hotkey);

            if (SuppressedKeys.Contains(hotkey))
                SuppressedKeys.Remove(hotkey);

            if (keyState == KeyState.Released)
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

        /// <summary>
        ///   Clear the actions for the hotkeys.
        /// </summary>
        public void ClearActions()
        {
            AllowedKeys.Clear();
            SuppressedKeys.Clear();
            KeyMappingsPressed.Clear();
            KeyMappingsReleased.Clear();
        }

        /// <summary>
        ///   Verify is key down using a <see cref="InputKey"/>.
        /// </summary>
        public bool IsKeyDown(InputKey key) => KeysState[key];

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

        /// <inheritdoc/>
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
