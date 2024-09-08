using InputHookManager.Enums;
using InputHookManager.Utils;
using System.Runtime.InteropServices;

namespace InputHookManager
{
    public partial class InputController
    {
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        public LowLevelKeyboardProc KeyboardProc;
        public IntPtr KeyboardId = IntPtr.Zero;

        public IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var vkCode = Marshal.ReadInt32(lParam);
            var keyCode = (KeyInput)vkCode;

            if (keyCode == KeyInput.LControlKey || keyCode == KeyInput.RControlKey || keyCode == KeyInput.ControlKey || keyCode == KeyInput.Control)
                KeyPressed.CtrlKeyPressed = true;
            else if (keyCode == KeyInput.LShiftKey || keyCode == KeyInput.RShiftKey || keyCode == KeyInput.ShiftKey || keyCode == KeyInput.Shift)
                KeyPressed.ShiftKeyPressed = true;
            else if (keyCode == KeyInput.LMenu || keyCode == KeyInput.RMenu || keyCode == KeyInput.Menu || keyCode == KeyInput.Alt)
                KeyPressed.AltKeyPressed = true;
            else
                KeyPressed.MainKey = keyCode;


            if (nCode >= 0 && (wParam == (IntPtr)WinMessages.WM_KEYDOWN || wParam == (IntPtr)WinMessages.WM_SYSKEYDOWN))  // WM_KEYDOWN
            {
                KeyStates[keyCode] = true;

                foreach (var hotKeyAction in KeyMappingsPressed)
                {
                    var isSubset = KeyPressed.Equals(hotKeyAction.Key);

                    if (isSubset)
                    {
                        if (AllowedKeys.Contains(KeyPressed))
                            hotKeyAction.Value.Invoke(KeyPressed.MainKey);
                        else if (WinUtils.IsActiveWindow(Hwnd)) //other key invoke just if app is visible
                        {
                            hotKeyAction.Value.Invoke(KeyPressed.MainKey);
                            return 1; // suppress_key
                        }
                    }
                }

                foreach (var hotKeyAction in KeyMappingsReleased)
                    if (KeyPressed.Equals(hotKeyAction.Key) && WinUtils.IsActiveWindow(Hwnd))
                        return 1;
            }
            else if (nCode >= 0 && (wParam == (IntPtr)WinMessages.WM_KEYUP || wParam == (IntPtr)WinMessages.WM_SYSKEYUP)) // WM_KEYUP
            {
                KeyStates[keyCode] = false;

                foreach (var hotKeyAction in KeyMappingsReleased)
                {
                    bool isSubset = KeyPressed.Equals(hotKeyAction.Key);

                    if (isSubset)
                    {
                        if (AllowedKeys.Contains(KeyPressed))
                        {
                            hotKeyAction.Value.Invoke(KeyPressed.MainKey);
                            break;
                        }
                        else if (WinUtils.IsActiveWindow(Hwnd))
                        {
                            hotKeyAction.Value.Invoke(KeyPressed.MainKey);
                            return 1; // suppress_key
                        }
                    }
                }

                KeyPressed.Clear();
            }

            return CallNextHookEx(KeyboardId, nCode, wParam, lParam);
        }

    }

}
