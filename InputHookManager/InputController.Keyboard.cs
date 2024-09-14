using InputHookManager.Enums;
using InputHookManager.Utils;
using System.Runtime.InteropServices;

namespace InputHookManager
{
    public partial class InputController
    {
        internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        internal LowLevelKeyboardProc KeyboardProc = default!;
        internal IntPtr KeyboardId = IntPtr.Zero;

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0 || !IsHookActive)
                return CallNextHookEx(KeyboardId, nCode, wParam, lParam);

            var vkCode = Marshal.ReadInt32(lParam);
            var keyCode = (InputKey)vkCode;
            UpdateKeyboardPressed(keyCode);

            bool isKeyDown = wParam == (IntPtr)WinMessages.WM_KEYDOWN || wParam == (IntPtr)WinMessages.WM_SYSKEYDOWN;
            bool isKeyUp = wParam == (IntPtr)WinMessages.WM_KEYUP || wParam == (IntPtr)WinMessages.WM_SYSKEYUP;
            bool actionResult = false;

            if (isKeyDown)
            {
                KeysState[keyCode] = true;
                actionResult = HandleKeyAction(KeyMappingsPressed);

                //suppress_invoke_action_release
                if (KeyMappingsReleased.ContainsKey(KeyPressed) && (WinUtils.IsActiveWindow(Hwnd) || Hwnd == 0 || AllowedKeys.Contains(KeyPressed)))
                    actionResult = true;
            }
            else if (isKeyUp)
            {
                KeysState[keyCode] = false;
                actionResult = HandleKeyAction(KeyMappingsReleased);
                KeyPressed.Clear();
            }

            if (actionResult && SuppressedKeys.Contains(KeyPressed) && !HotKey.IsCommandKey(keyCode))
                return 1;

            return CallNextHookEx(KeyboardId, nCode, wParam, lParam);
        }

        private bool HandleKeyAction(Dictionary<HotKey, Action<object>> keyMappings)
        {
            foreach (var hotKeyAction in keyMappings)
            {
                if (KeyPressed.Equals(hotKeyAction.Key))
                {
                    if (AllowedKeys.Contains(KeyPressed) || Hwnd == 0)
                    {
                        hotKeyAction.Value.Invoke(KeyPressed);
                        return true;
                    }
                    if (WinUtils.IsActiveWindow(Hwnd))
                    {
                        hotKeyAction.Value.Invoke(KeyPressed);
                        return true;
                    }
                }
            }

            return false;
        }

        private void UpdateKeyboardPressed(InputKey keyCode)
        {
            if (HotKey.IsControlKey(keyCode))
                KeyPressed.CtrlKeyPressed = true;
            else if (HotKey.IsShiftKey(keyCode))
                KeyPressed.ShiftKeyPressed = true;
            else if (HotKey.IsAltKey(keyCode))
                KeyPressed.AltKeyPressed = true;
            else
            {
                KeyPressed.MainKey = keyCode;
                return;
            }

            KeyPressed.MainKey = InputKey.None;
        }

    }

}
