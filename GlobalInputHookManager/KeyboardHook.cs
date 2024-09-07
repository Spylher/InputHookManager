using GlobalInputHookManager.Utils;
using System.Runtime.InteropServices;
using GlobalInputHookManager.Enums;

namespace GlobalInputHookManager
{
    using static InputHookManager;
    using static WinMessages;
    using static WindowUtils;

    public class KeyboardHook
    {
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        public static LowLevelKeyboardProc Proc = KeyboardHookCallback;
        public static IntPtr Id = IntPtr.Zero;

        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var vkCode = Marshal.ReadInt32(lParam);
            var keyCode = (Keys)vkCode;

            if (keyCode == Keys.LControlKey || keyCode == Keys.RControlKey || keyCode == Keys.ControlKey || keyCode == Keys.Control)
                KeyPressed.CtrlKeyPressed = true;
            else if (keyCode == Keys.LShiftKey || keyCode == Keys.RShiftKey || keyCode == Keys.ShiftKey || keyCode == Keys.Shift)
                KeyPressed.ShiftKeyPressed = true;
            else if (keyCode == Keys.LMenu || keyCode == Keys.RMenu || keyCode == Keys.Menu || keyCode == Keys.Alt)
                KeyPressed.AltKeyPressed = true;
            else
                KeyPressed.MainKey = keyCode;


            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))  // WM_KEYDOWN
            {
                KeyStates[keyCode] = true;

                foreach (var hotKeyAction in KeyMappingsPressed)
                {
                    var isSubset = KeyPressed.Equals(hotKeyAction.Key);

                    if (isSubset)
                    {
                        if (AllowedKeys.Contains(KeyPressed))
                            hotKeyAction.Value.Invoke(KeyPressed.MainKey);
                        else if (IsActiveWindow(Hwnd)) //other key invoke just if app is visible
                        {
                            hotKeyAction.Value.Invoke(KeyPressed.MainKey);
                            return 1; // suppress_key
                        }
                    }
                }

                foreach (var hotKeyAction in KeyMappingsReleased)
                    if (KeyPressed.Equals(hotKeyAction.Key) && IsActiveWindow(Hwnd))
                        return 1;
            }
            else if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)) // WM_KEYUP
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
                        else if (IsActiveWindow(Hwnd))
                        {
                            hotKeyAction.Value.Invoke(KeyPressed.MainKey);
                            return 1; // suppress_key
                        }
                    }
                }

                KeyPressed.Clear();
            }

            return CallNextHookEx(Id, nCode, wParam, lParam);
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    }
}
