﻿using InputHookManager.Enums;
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

        public InputController()
        {
            Task.Run(() =>
            {
                //keyboard
                KeyboardProc = KeyboardHookCallback;
                KeyboardId = SetKeyboardHook(KeyboardProc);

                //mouse
                MouseProc = MouseHookCallback;
                MouseHookId = SetMouseHook(MouseProc);

                //Enable();
                foreach (InputKey key in Enum.GetValues(typeof(InputKey)))
                    KeyState[key] = false;

                CaptureMessages();
            });
        }

        public void CaptureMessages()
        {
            while (GetMessage(out WinUtils.MESSAGE msg, IntPtr.Zero, 0, 0))
            {
                //Thread.Sleep(1);
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

        public void Attach(IntPtr hwnd) => Hwnd = hwnd;

        public void RegisterAction(HotKey hotkey, Action<Object> act, bool suppressDefault = false, ActionMode actionMode = ActionMode.Default, KeyState keyState = Enums.KeyState.Released)
        {
            if (keyState == Enums.KeyState.Pressed)
            {
                UnregisterAction(hotkey, Enums.KeyState.Pressed);
                KeyMappingsPressed.Add(hotkey, act);
            }
            else if (keyState == Enums.KeyState.Released)
            {
                UnregisterAction(hotkey);
                KeyMappingsReleased.Add(hotkey, act);
            }

            if (actionMode == ActionMode.Global)
                AllowedKeys.Add(hotkey);
            if (suppressDefault)
                SuppressedKeys.Add(hotkey);
        }

        public void UnregisterAction(HotKey hotkey, KeyState keyState = Enums.KeyState.Released)
        {
            if (AllowedKeys.Contains(hotkey)) AllowedKeys.Remove(hotkey);
            if (SuppressedKeys.Contains(hotkey)) SuppressedKeys.Remove(hotkey);

            if (keyState == Enums.KeyState.Pressed)
            {
                foreach (var keyValuePair in KeyMappingsPressed)
                    if (keyValuePair.Key.Equals(hotkey))
                        KeyMappingsPressed.Remove(hotkey);

            }
            else if (keyState == Enums.KeyState.Released)
            {
                foreach (var keyValuePair in KeyMappingsReleased)
                    if (keyValuePair.Key.Equals(hotkey))
                        KeyMappingsReleased.Remove(hotkey);
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
            ClearActions();
            //Disable();
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
        public static extern bool GetMessage(out WinUtils.MESSAGE lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    }

}
