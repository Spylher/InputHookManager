using GlobalInputHookManager.Utils;
using MySpy.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlobalInputHookManager
{
    using static InputHookManager;
    using static WinMessages;
    using static WindowUtils;

    public class KeyboardHook
    {
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        public static LowLevelKeyboardProc KeyboardProc = KeyboardHookCallback;
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

                foreach (var HK in KeyMappingsPressed)
                {
                    bool isSubset = KeyPressed.Equals(HK.Key);

                    if (isSubset)
                    {
                        if (AllowedKeys.Contains(KeyPressed))
                            HK.Value.Invoke(KeyPressed.MainKey);
                        else if (IsActiveWindow(HWND)) //other key invoke just if app is visible
                        {
                            HK.Value.Invoke(KeyPressed.MainKey);
                            return (IntPtr)1; // suppress_key
                        }
                    }
                }

                foreach (var HK in KeyMappingsReleased)
                    if (KeyPressed.Equals(HK.Key) && IsActiveWindow(HWND))
                        return (IntPtr)1;
            }
            else if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)) // WM_KEYUP
            {
                KeyStates[keyCode] = false;

                foreach (var HK in KeyMappingsReleased)
                {
                    //MessageBox.Show($"pressed{KeyPressed},  list {HK.Key}");
                    bool isSubset = KeyPressed.Equals(HK.Key);

                    if (isSubset)
                    {
                        //keyStart -> invoke function of keystart
                        //if (KeyStart.MainKey.ToString() == KeyPressed.ToString())
                        if (AllowedKeys.Contains(KeyPressed))
                        {
                            HK.Value.Invoke(KeyPressed.MainKey);
                            break;
                        }
                        else if (IsActiveWindow(HWND))
                        {
                            HK.Value.Invoke(KeyPressed.MainKey);
                            return (IntPtr)1; // suppress_key
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
