using InputHookManager.Enums;
using System.Runtime.InteropServices;
using static InputHookManager.Utils.WinUtils;
using static InputHookManager.Enums.WinMessages;

namespace InputHookManager
{
    public partial class InputController
    {
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelMouseProc MouseProc;
        private IntPtr MouseHookId = IntPtr.Zero;

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT))!;
            int buttonValue = (int)((hookStruct.mouseData >> 16) & 0xFFFF);

            if (buttonValue == 1)
                KeyPressed.MainKey = KeyInput.XButton1;
            else
                KeyPressed.MainKey = KeyInput.XButton2;


            if (nCode >= 0 && (wParam == (IntPtr)WM_LBUTTONDOWN || wParam == (IntPtr)WM_RBUTTONDOWN || wParam == (IntPtr)WM_XBUTTONDOWN))
            {
                if (wParam == (IntPtr)WM_XBUTTONDOWN)
                {
                    foreach (var HK in KeyMappingsPressed)
                    {
                        bool isSubset = KeyPressed.Equals(HK);

                        if (isSubset)
                        {
                            ////keyStart -> invoke function of keystart
                            //if (KeyStart.MainKey.ToString() == KeyPressed.ToString())
                            //{
                            //    HK.Value.Invoke(KeyPressed.MainKey);
                            //    break;
                            //}
                            //else if (IsActiveWindow(Hwnd)) //other key invoke just if app is visible
                            //{
                            //    HK.Value.Invoke(KeyPressed.MainKey);
                            //    //return (IntPtr)1; // suppress_key
                            //}
                        }
                    }
                }

            }
            else if (nCode >= 0 && (wParam == (IntPtr)WM_LBUTTONUP || wParam == (IntPtr)WM_RBUTTONUP || wParam == (IntPtr)WM_XBUTTONUP))
            {
                if (wParam == (IntPtr)WM_XBUTTONUP)
                {
                    foreach (var HK in KeyMappingsReleased)
                    {
                        bool isSubset = KeyPressed.Equals(HK);

                        if (isSubset)
                        {
                            ////keyStart -> invoke function of keystart
                            //if (KeyStart.MainKey.ToString() == KeyPressed.ToString())
                            //{
                            //    HK.Value.Invoke(KeyPressed.MainKey);
                            //    break;
                            //}
                            //else if (IsActiveWindow(Hwnd)) //other key invoke just if app is visible
                            //{
                            //    HK.Value.Invoke(KeyPressed.MainKey);
                            //    //return (IntPtr)1; // suppress_key
                            //}
                        }
                    }
                }

                KeyPressed.Clear();
            }

            return CallNextHookEx(MouseHookId, nCode, wParam, lParam);
        }

    }

}
