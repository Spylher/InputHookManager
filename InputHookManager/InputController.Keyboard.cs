using InputHookManager.Enums;
using InputHookManager.Utils;
using System.Runtime.InteropServices;

namespace InputHookManager
{
    public partial class InputController
    {
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelKeyboardProc KeyboardProc;
        private IntPtr KeyboardId = IntPtr.Zero;

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return CallNextHookEx(KeyboardId, nCode, wParam, lParam);

            var vkCode = Marshal.ReadInt32(lParam);
            var keyCode = (KeyInput)vkCode;
            UpdateKeyPressed(keyCode);

            bool isKeyDown = wParam == (IntPtr)WinMessages.WM_KEYDOWN || wParam == (IntPtr)WinMessages.WM_SYSKEYDOWN;
            bool isKeyUp = wParam == (IntPtr)WinMessages.WM_KEYUP || wParam == (IntPtr)WinMessages.WM_SYSKEYUP;
            bool actionResult = false;

            if (isKeyDown)
            {
                KeyState[keyCode] = true;
                actionResult = HandleKeyAction(KeyMappingsPressed);

                //suppress_invoke_action_release
                if (KeyMappingsReleased.ContainsKey(KeyPressed) && (WinUtils.IsActiveWindow(Hwnd) || AllowedKeys.Contains(KeyPressed)))
                    actionResult = true;
            }
            else if (isKeyUp)
            {
                KeyState[keyCode] = false;
                actionResult = HandleKeyAction(KeyMappingsReleased);
                KeyPressed.Clear();
            }

            if (actionResult && SuppressedKeys.Contains(KeyPressed))
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
                        hotKeyAction.Value.Invoke(KeyPressed.MainKey);
                        return true;
                    }
                    if (WinUtils.IsActiveWindow(Hwnd))
                    {
                        hotKeyAction.Value.Invoke(KeyPressed.MainKey);
                        return true;
                    }
                }
            }

            return false;
        }

        private void UpdateKeyPressed(KeyInput keyCode)
        {
            if (IsControlKey(keyCode))
                KeyPressed.CtrlKeyPressed = true;
            else if (IsShiftKey(keyCode))
                KeyPressed.ShiftKeyPressed = true;
            else if (IsAltKey(keyCode))
                KeyPressed.AltKeyPressed = true;
            else
                KeyPressed.MainKey = keyCode;
        }

        private bool IsControlKey(KeyInput keyCode)
        {
            return (keyCode == KeyInput.LControlKey || keyCode == KeyInput.RControlKey || keyCode == KeyInput.ControlKey || keyCode == KeyInput.Control);
        }

        private bool IsShiftKey(KeyInput keyCode)
        {
            return (keyCode == KeyInput.LShiftKey || keyCode == KeyInput.RShiftKey || keyCode == KeyInput.ShiftKey || keyCode == KeyInput.Shift);
        }

        private bool IsAltKey(KeyInput keyCode)
        {
            return (keyCode == KeyInput.LMenu || keyCode == KeyInput.RMenu || keyCode == KeyInput.Menu || keyCode == KeyInput.Alt);
        }

    }

}
