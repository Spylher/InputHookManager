using InputHookManager.Enums;
using InputHookManager.Utils;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InputHookManager
{
    public partial class InputManager : IDisposable
    {
        internal Dictionary<InputKey[], Action<object>> KeyMappingsPressed = new(new InputKeyArrayComparer());
        internal Dictionary<InputKey[], Action<object>> KeyMappingsReleased = new(new InputKeyArrayComparer());
        internal static Dictionary<InputKey, bool> KeyStates = [];
        internal HashSet<InputKey[]> GlobalKeys = new(new InputKeyArrayComparer());
        internal HashSet<InputKey[]> SuppressedKeys = new(new InputKeyArrayComparer());
        internal ISimulator Simulator = new WinApi();
        internal bool ActionInProgress = false;
        internal bool IsHookActive = true;
        public IntPtr Hwnd = IntPtr.Zero;

        /// <summary>
        ///     Initialize a <see cref="InputManager"/>.
        /// </summary>
        /// <param name="executionMode">, execution mode</param>
        /// .
        public InputManager(ExecutionMode executionMode = ExecutionMode.UserMode)
        {
            Enable();

            if (executionMode == ExecutionMode.KernelMode)
            {
                var keyboardPath = Path.Combine(Environment.SystemDirectory, "drivers", "keyboard.sys");
                var mousePath = Path.Combine(Environment.SystemDirectory, "drivers", "mouse.sys");

                if (!File.Exists(keyboardPath) || !File.Exists(mousePath))
                    throw new Exception("Interception Driver not detected, type InputManager.InstallInterception and reboot your system.");

                Simulator = new DriverApi();

                Interception.CancelableOnKeyDown += KeyboardDriverCallback_OnKeyDown;
                Interception.CancelableOnKeyUp += KeyboardDriverCallback_OnKeyUp;
                Interception.CancelableOnMouseMove += OnMouseMove;

                // To do
                // This is a workaround for the Interception driver to handle mouse movement events.
                static bool OnMouseMove(int x, int y)
                {
                    //if (InputManager.IsKeyDown(InputKey.C))
                    //Interception.MoveMouse(0, 0);
                    //Interception.SetMouse(0, 0);
                    return true;
                }
            }
            else
                Task.Run(InitializeHooks);
        }

        public static void InstallInterception() => Driver.Program.InstallInterception();

        public static void UninstallInterception() => Driver.Program.UninstallInterception();

        private void CaptureMessages()
        {
            while (GetMessage(out _, IntPtr.Zero, 0, 0))
            {
                Thread.Sleep(1);
            }
        }

        private void InitializeHooks()
        {
            //keyboard
            KeyboardProc = KeyboardHookCallback;
            KeyboardHookId = SetKeyboardHook(KeyboardProc);

            //mouse
            MouseProc = MouseHookCallback;
            MouseHookId = SetMouseHook(MouseProc);

            CaptureMessages();
        }

        public void Update(object sender)
        {
            var handler = sender switch
            {
                ProcessInfo procInfo => procInfo.MainWindowHandle,
                Process proc => proc.MainWindowHandle,
                IntPtr hwnd => hwnd,
                _ => throw new ArgumentException("Invalid sender type. Expected ProcessInfo, Process or hwnd.")
            };

            Hwnd = handler;
        }

        /// <summary>
        ///     Enable the hook.
        /// </summary>
        public void Enable()
        {
            IsHookActive = true;

            foreach (InputKey key in Enum.GetValues(typeof(InputKey)))
                KeyStates[key] = false;
        }

        /// <summary>
        ///     Disable the hook.
        /// </summary>
        public void Disable() => IsHookActive = false;

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

        public void RegisterAction(InputKey[] keys, Action<object> act, KeyMode keyState = KeyMode.Pressed, bool suppressDefault = true, ActionMode actionMode = ActionMode.Windowed)
        {
            if (keys.Contains(InputKey.None))
                throw new Exception($"Cannot register None key");
            if (keys.Length != keys.Distinct().Count())
                throw new Exception($"There is duplicates, InputKey is Invalid");

            if (keyState == KeyMode.Released)
            {
                UnregisterAction(keys, KeyMode.Released);
                KeyMappingsReleased.Add(keys, act);
            }
            else
            {
                UnregisterAction(keys);
                KeyMappingsPressed.Add(keys, act);
            }

            KeyMappingsPressed.Order();

            if (actionMode == ActionMode.Global)
                GlobalKeys.Add(keys);
            if (suppressDefault)
                SuppressedKeys.Add(keys);
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
        ///     Use <see cref="KeyMode.Pressed"/> for the start action while key is pressed,
        ///     or <see cref="KeyMode.Released"/> to the start action when key is released,
        /// </param>.
        public void RegisterAction(HotKey hotkey, Action<object> act, KeyMode keyState = KeyMode.Pressed, bool suppressDefault = true, ActionMode actionMode = ActionMode.Windowed)
        {
            var keys = hotkey.ToInputKey();
            RegisterAction(keys, act, keyState, suppressDefault, actionMode);
        }

        public void RegisterAction(InputKey key, Action<object> act, KeyMode keyState = KeyMode.Pressed, bool suppressDefault = true, ActionMode actionMode = ActionMode.Windowed)
        {
            RegisterAction([key], act, keyState, suppressDefault, actionMode);
        }

        /// <summary>
        ///     Unregister the action for the shortcut using a <see cref="HotKey"/>.
        /// </summary>
        /// <param name="hotkey"> Shortcut entity </param>.
        /// <param name="keyState"> Specifies key state to remove of the action </param>.
        public void UnregisterAction(HotKey hotkey, KeyMode keyState = KeyMode.Pressed)
        {
            var hk = hotkey.ToInputKey();
            UnregisterAction(hk, keyState);
        }

        public void UnregisterAction(InputKey[] keys, KeyMode keyState = KeyMode.Pressed)
        {
            if (GlobalKeys.Contains(keys))
                GlobalKeys.Remove(keys);

            if (SuppressedKeys.Contains(keys))
                SuppressedKeys.Remove(keys);

            if (keyState == KeyMode.Released)
            {
                if (KeyMappingsReleased.ContainsKey(keys))
                    KeyMappingsReleased.Remove(keys);
            }
            else
            {
                if (KeyMappingsPressed.ContainsKey(keys))
                    KeyMappingsPressed.Remove(keys);
            }
        }

        public void UnregisterAction(InputKey key, KeyMode keyState = KeyMode.Pressed)
        {
            UnregisterAction([key], keyState);
        }

        /// <summary>
        ///   Clear the actions for the hotkeys.
        /// </summary>
        public void ClearActions()
        {
            GlobalKeys.Clear();
            SuppressedKeys.Clear();
            KeyMappingsPressed.Clear();
            KeyMappingsReleased.Clear();
        }

        /// <summary>
        ///   Verify is key down using a <see cref="InputKey"/>.
        /// </summary>

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
            UnhookWindowsHookEx(KeyboardHookId);
            UnhookWindowsHookEx(MouseHookId);
            Interception.CancelableOnKeyDown -= KeyboardDriverCallback_OnKeyDown;
            Interception.CancelableOnKeyUp -= KeyboardDriverCallback_OnKeyUp;
        }


        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern bool GetMessage(out WinApi.MESSAGE lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    }

}
